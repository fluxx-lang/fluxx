using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Faml.DotNet;
using Faml.VisualStudio.VsUtil;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using TypeTooling.DotNet.RawTypes;
using TypeTooling.DotNet.RawTypes.Roslyn;
using Timer = System.Timers.Timer;

namespace Faml.VisualStudio {
    public class RoslynDotNetRawTypeProvider : DotNetRawTypeProvider, IDisposable {
        private static readonly TimeSpan UpdateCompletionDelay = TimeSpan.FromSeconds(1);

        private readonly VisualStudioWorkspace _roslynWorkspace;
        private readonly IVsProject _vsProject;
        private bool _isReady;
        private ProjectId _roslynProjectId;
        private Compilation _fallbackCompilation;
        private readonly Timer _notifyTimer = new Timer();
        private bool? _pendingIsEntireProjectAffected;
        private readonly ConcurrentDictionary<string, Assembly> _loadedAssemblies = new ConcurrentDictionary<string, Assembly>();

        public event EventHandler<CompilationChangedArgs> CompilationChanged;

        private bool IsDisposed { get; set; }
        private Compilation LastReadyCompilation { get; set; }
        public override bool IsReady => _isReady;

        public RoslynDotNetRawTypeProvider(IVsProject vsProject) {
            _roslynWorkspace = FamlPackage.Instance.RoslynWorkspace;
            _vsProject = vsProject;
            _roslynProjectId = null;

            _notifyTimer.AutoReset = false;
            _notifyTimer.Interval = UpdateCompletionDelay.TotalMilliseconds;
            _notifyTimer.Elapsed += OnNotifyTimer;

            Initialize();
        }

        public override DotNetRawType? GetType(string typeName) {
            Compilation compilation = CurrentCompilation;

            // TODO: Maybe support EnsureFallbackCompilation
            if (compilation == null)
                return null;

            INamedTypeSymbol objectType = compilation.GetTypeByMetadataName("System.Object");
            INamedTypeSymbol stringType = compilation.GetTypeByMetadataName("System.String");
            INamedTypeSymbol command = compilation.GetTypeByMetadataName("Xamarin.Forms.ICommand");

            INamedTypeSymbol symbol = compilation.GetTypeByMetadataName(typeName);
            if (symbol == null)
                return null;

            return new RoslynDotNetRawType(symbol);
        }

        public override IReadOnlyList<DotNetRawType> GetAssemblyAttributeReferencedTypes(string attributeFullName) {
            string attributeName = attributeFullName.Substring(attributeFullName.LastIndexOf('.') + 1);

            List<DotNetRawType> types = new List<DotNetRawType>();

            Compilation compilation = CurrentCompilation;

            foreach (MetadataReference reference in compilation.References) {
                ISymbol symbol = compilation.GetAssemblyOrModuleSymbol(reference);

                if (symbol is IAssemblySymbol assemblySymbol) {
                    ImmutableArray<AttributeData> attributes = assemblySymbol.GetAttributes();

                    foreach (AttributeData attributeData in attributes) {
                        INamedTypeSymbol attributeClass = attributeData.AttributeClass;

                        // As an optimization, match on the simple name first to avoid computing the full name
                        // for everything
                        if (attributeClass.Name != attributeName)
                            continue;
                        if (attributeClass.GetFullName() != attributeFullName)
                            continue;

                        // The type needs to be the first and only argument
                        ImmutableArray<TypedConstant> constructorArguments = attributeData.ConstructorArguments;
                        if (constructorArguments.Length != 1)
                            continue;

                        TypedConstant argument = constructorArguments[0];
                        if (! (argument.Value is INamedTypeSymbol namedTypeSymbol))
                            continue;

                        RoslynDotNetRawType type = new RoslynDotNetRawType(namedTypeSymbol);
                        types.Add(type);
                    }
                }
            }

            return types.AsReadOnly();
        }

        public override async Task<IEnumerable<DotNetRawType>> FindTypesAssignableToAsync(DotNetRawType rawType, CancellationToken cancellationToken) {
            var roslynType = (RoslynDotNetRawType)rawType;

            if (!(roslynType.TypeSymbol is INamedTypeSymbol namedTypeSymbol))
                return Enumerable.Empty<DotNetRawType>();

            IEnumerable<INamedTypeSymbol> derivedClasses = await SymbolFinder.FindDerivedClassesAsync(namedTypeSymbol,
                _roslynWorkspace.CurrentSolution, null, cancellationToken);

            List<DotNetRawType> returnedTypes = new List<DotNetRawType>();
            foreach (INamedTypeSymbol derivedClass in derivedClasses)
                returnedTypes.Add(new RoslynDotNetRawType(derivedClass));

            return returnedTypes;
        }

        public override object Instantiate(DotNetRawType type, params object[] args) {
            var roslynType = (RoslynDotNetRawType) type;

            IAssemblySymbol assemblySymbol = roslynType.TypeSymbol.ContainingAssembly;

            MetadataReference assemblyMetadataReference = LastReadyCompilation.GetMetadataReference(assemblySymbol);
            if (assemblyMetadataReference is PortableExecutableReference || assemblyMetadataReference is CompilationReference) {
                string assemblyPath = assemblyMetadataReference.Display;

                if (!_loadedAssemblies.TryGetValue(assemblyPath, out Assembly assembly)) {
                    assembly = LoadAssembly(assemblyPath);
                    assembly = _loadedAssemblies.GetOrAdd(assemblyPath, assembly);
                }

                Type reflectionType = assembly.GetType(roslynType.FullName);

                return Activator.CreateInstance(reflectionType, args);
            }
            else throw new InvalidOperationException($"Can't load assembly for {assemblySymbol} as Roslyn metadata reference type {assemblyMetadataReference.GetType()} isn't supported");
        }

        private Assembly LoadAssembly(string assemblyPath) {
            if (_loadedAssemblies.TryGetValue(assemblyPath, out Assembly assembly))
                return assembly;

            try {
                Assembly loadedAssembly = Assembly.LoadFrom(assemblyPath);
                return _loadedAssemblies.GetOrAdd(assemblyPath, loadedAssembly);
            }
            catch (Exception e) {
                throw e;
            }
        }

        public void Initialize() {
            // If Roslyn project is not available then we'll need to wait till solution loads
            if (TryGetReady())
                _pendingIsEntireProjectAffected = null;
            else if (!KnownUIContexts.SolutionExistsAndFullyLoadedContext.IsActive) {
                KnownUIContexts.SolutionExistsAndFullyLoadedContext.PropertyChanged +=
                    OnSolutionExistsAndFullyLoadedContext;
            }

            // Initialization might happen while building or debugging. If we fail to get
            // ready then we'll need to retry next time when we exit building/debugging.
            KnownUIContexts.DebuggingContext.PropertyChanged += OnDebuggingContextChanged;
            KnownUIContexts.SolutionBuildingContext.PropertyChanged += OnSolutionBuildingContextChanged;
        }

        private void OnSolutionExistsAndFullyLoadedContext(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(UIContext.IsActive) &&
                KnownUIContexts.SolutionExistsAndFullyLoadedContext.IsActive) {
                KnownUIContexts.SolutionExistsAndFullyLoadedContext.PropertyChanged -=
                    OnSolutionExistsAndFullyLoadedContext;
                TryStartNotifyTimer(isEntireProjectAffected: true);
            }
        }

        private bool InitializeProject() {
            if (_roslynProjectId != null)
                return true;

            // This method can be called multiple times as we try to get ready e.g. while
            // loading solution. We need to make sure we subscribe only once.
            _roslynWorkspace.WorkspaceChanged -= OnWorkspaceChanged;
            _roslynWorkspace.WorkspaceChanged += OnWorkspaceChanged;

            Debug.Assert(!IsDisposed);

            Project project = GetRoslynProjectForHierarchy();
            _roslynProjectId = project?.Id;

            if (_roslynProjectId != null) {
#if MAYBE_LATER
                this.WatchReferences();
#endif

                return true;
            }
            else
                return false;
        }

        private Project GetRoslynProjectForHierarchy() {
            string projectFilePath = _vsProject.GetFilePath();

            // We can see up to 2 Roslyn projects. One is "classic" and another is special XAML. In
            // theory there might be more. For now we'll use first project which can provide Compilation.
            List<Project> projects = _roslynWorkspace.CurrentSolution.Projects.Where(proj =>
                proj.SupportsCompilation && proj.FilePath.Equals(projectFilePath, StringComparison.Ordinal)).ToList();
            if (projects.Count > 1)
                throw new InvalidOperationException("Multiple projects are available. We need to improve logic");
            return projects.FirstOrDefault();
        }

        private void OnWorkspaceChanged(object sender, WorkspaceChangeEventArgs e) {
            System.Diagnostics.Debug.WriteLine($"OnWorkspaceChanged: {e.Kind}");

            if (_roslynProjectId == null && e.Kind == WorkspaceChangeKind.ProjectAdded) {
                Project project = GetRoslynProjectForHierarchy();
                _roslynProjectId = project?.Id;
                if (_roslynProjectId != null) {
#if MAYBE_LATER
                    this.WatchReferences();
#endif
                }
            }

            // WorkspaceChanged event fires frequently (thus use of timer):
            //   * On each key stroke when editing source files in "this" project
            //   * When build is in progress
            // ISSUE. WorkspaceChanged does not fire at all when editing source files in referenced projects.
            if (e.ProjectId == _roslynProjectId && e.Kind != WorkspaceChangeKind.ProjectRemoved) {
                WorkspaceChangeKind kind = e.Kind;
                bool isEntireProjectAffected = kind == WorkspaceChangeKind.ProjectAdded ||
                                               kind == WorkspaceChangeKind.ProjectChanged ||
                                               kind == WorkspaceChangeKind.ProjectReloaded;

                TryStartNotifyTimer(isEntireProjectAffected);
            }
        }

        private Project GetRoslynProject() => _roslynWorkspace.CurrentSolution.GetProject(_roslynProjectId);

        public async Task<Compilation> GetCompilationAsync() {
            if (_isReady) {
                Project roslynProject = GetRoslynProject();
                Compilation compilation = await GetCompilationIfReadyAsync(roslynProject).ConfigureAwait(false);
                if (compilation == null) {
                    // If we managed to get into a state where new copilation has no Object type
                    // we want to stick to last "ready" compilation. This can happen due to
                    // ongoing issues with NuGet ingrastructure.
                    compilation = LastReadyCompilation;
                }
                else
                    LastReadyCompilation = compilation;

                if (compilation != null)
                    return compilation;

                Debug.Fail("Why there is no last ready compilation");
                _isReady = false;
            }

            EnsureFallbackCompilation();
            return _fallbackCompilation;
        }

        public Compilation CurrentCompilation {
            get {
                Compilation compilation = LastReadyCompilation;
                if (compilation == null)
                    compilation = EnsureFallbackCompilation();

                return compilation;
            }
        }

        private void OnDebuggingContextChanged(object sender, PropertyChangedEventArgs e) {
            // Try to notify when debug session ends
            if (e.PropertyName == nameof(UIContext.IsActive) &&
                !KnownUIContexts.DebuggingContext.IsActive) {
                TryNotifyCompilationChanged();
            }
        }

        private void OnSolutionBuildingContextChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(UIContext.IsActive) &&
                !KnownUIContexts.SolutionBuildingContext.IsActive) {
                // Prepare to notify even if types did not change
#if NOT_NEEDED
                if (ShouldNotifyOnNonCompilationEvents)
                    TryStartNotifyTimer(isEntireProjectAffected: true);
#endif

                // Try to notify when build completes
                TryNotifyCompilationChanged();
            }
        }

        private bool TryGetReady() {
            if (_isReady)
                return true;

            try {
                if (!InitializeProject())
                    return false;

#if MAYBE_LATER
                if (Interlocked.CompareExchange(ref this.listeningTo/hanged, 1, 0) == 0) {
                    this.HostProject.ReferencesChanged += this.OnProjectReferencesChanged;
                }
#endif

                Project roslynProject = _roslynWorkspace.CurrentSolution.GetProject(_roslynProjectId);
                Task<Compilation> task = GetCompilationIfReadyAsync(roslynProject);
                if (task.Status != TaskStatus.RanToCompletion) {
                    // Retry and notify as soon as compilation is ready. 
                    task.ContinueWith(t =>
                            OnCompilationReady(t.Result, shouldNotify: true),
                        TaskContinuationOptions.OnlyOnRanToCompletion);
                }
                else
                    OnCompilationReady(task.Result, shouldNotify: false);
            }
            catch {
                return false;
            }

            lock (_notifyTimer) {
                // If not ready, retry after some time
                if (!_isReady)
                    TryStartNotifyTimer(isEntireProjectAffected: true);
            }

            return _isReady;
        }

        internal void OnCompilationReady(Compilation compilation, bool shouldNotify) {
            bool gotReady = false;
            lock (_notifyTimer) {
                if (!IsDisposed && !_isReady && compilation != null) {
                    LastReadyCompilation = compilation;
                    _isReady = true;
                    _pendingIsEntireProjectAffected = true;
                    gotReady = true;
                }
            }

            if (shouldNotify && gotReady)
                StopTimerAndTryNotifyCompilationChanged();
        }

        private static async Task<Compilation> GetCompilationIfReadyAsync(Project roslynProject) {
            // Compilation is not ready if there is no Object type
            Compilation? compilation = null;
            if (roslynProject != null)
                compilation = await roslynProject.GetCompilationAsync().ConfigureAwait(false);

            INamedTypeSymbol objectSymbol = compilation?.ObjectType;
            if (objectSymbol != null && objectSymbol.TypeKind != TypeKind.Error)
                return compilation;

            return null;
        }

        protected void TryStartNotifyTimer(bool isEntireProjectAffected) {
            if (IsDisposed)
                return;

            lock (_notifyTimer) {
                _notifyTimer.Stop(); // restart timer
                if (isEntireProjectAffected || !_pendingIsEntireProjectAffected.HasValue)
                    _pendingIsEntireProjectAffected = isEntireProjectAffected;

                // Do not notify while build or debugging is in progress.
                if (!KnownUIContexts.SolutionBuildingContext.IsActive && !KnownUIContexts.DebuggingContext.IsActive)
                    _notifyTimer.Start();
            }
        }

        private void OnNotifyTimer(object sender, ElapsedEventArgs e) {
            StopTimerAndTryNotifyCompilationChanged();
        }

        private void StopTimerAndTryNotifyCompilationChanged() {
            lock (_notifyTimer)
                _notifyTimer.Stop();

#if NON_UI_THREAD_OK_I_THINK
            if (this.ShouldNotifyOnUIThread) {
                UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.ApplicationIdle, () =>
                    TryNotifyCompilationChanged());
            }
#endif
            TryNotifyCompilationChanged();
        }

        internal void TryNotifyCompilationChanged() {
            if (IsDisposed)
                return;

            BeforeNotifyCompilationChanged();
            if (!TryGetReady())
                return;

            bool isEntireProjectAffected;
            lock (_notifyTimer) {
                _notifyTimer.Stop();
                if (!_pendingIsEntireProjectAffected.HasValue) {
                    return;
                }

                isEntireProjectAffected = _pendingIsEntireProjectAffected.Value;
                _pendingIsEntireProjectAffected = null;
            }

            CompilationChanged?.Invoke(this, new CompilationChangedArgs(isEntireProjectAffected));
        }

        protected virtual void BeforeNotifyCompilationChanged() {
        }

#if MAYBE_LATER
        protected void WatchFile(string filePath) {
            // Do not watch files in installed locations. Note that file might not
            // exist yet, e.g. when adding a reference to an unbuilt project. We
            // should start watching anyway. So that we pick it up when it's built.
            if (!AssemblyHelper.IsInstalledAssembly(filePath, includeNuGetRoot: true))
                this.hostFileChangeWatcher.WatchFile(filePath);
        }

        protected void StopWatchingFile(string filePath) {
            this.hostFileChangeWatcher.StopWatchingFile(filePath);
        }

        private void HostFileChangeWatcher_FileChanged(object sender, FileSystemEventArgs e) {
            OnFileChanged(e);
        }
#endif

#if MAYBE_LATER
        private void OnProjectReferencesChanged(object sender, HostItemChangesEventArgs<HostReferenceData> e) {
            foreach (HostItemChange<HostReferenceData> change in e.Changes) {
                switch (change.ChangeType) {
                    case HostItemChangeType.ItemAdded:
                        ReferencedProjectOutputChanged(change.ChangedItem.Path, null);
                        break;
                    case HostItemChangeType.ItemRemoved:
                        ReferencedProjectOutputChanged(null, change.ChangedItem.Path);
                        break;
                    case HostItemChangeType.ItemChanged:
                        ReferencedProjectOutputChanged(change.ChangedItem.Path, change.ChangedItem.Path);
                        break;
                    case HostItemChangeType.ItemRenamed:
                        // Old name tends to be URI like file:///C:/MyApp/MyLib.dll. We want to change it to C:\myApp\MyLib.dll
                        string oldPath = new Uri(change.OldName).LocalPath;
                        ReferencedProjectOutputChanged(change.ChangedItem.Path, oldPath);
                        break;
                }
            }
        }
#endif

        private Compilation EnsureFallbackCompilation() {
            return null;

            if (_fallbackCompilation == null) {
                // Minimum set of "core" types needed for XAML diagnostics and designer to be happy. Note
                // that Roslyn's set of core types is bigger. If we find any issue then consider matching
                // https://github.com/dotnet/roslyn/blob/master/src/Compilers/Core/Portable/SpecialTypes.cs
                const string mscorlibStub = @"
namespace System
{
    public class Object {}
    public class String : Object {}
    public class Uri : Object {}
    public abstract class Array : Object, System.Collections.IList {}
    public abstract class Attribute : Object {}
    public abstract class Type : Object {}

    public struct Char {}
    public struct Byte {}
    public struct Boolean {}
    public struct Int16 {}
    public struct UInt16 {}
    public struct Int32 {}
    public struct UInt32 {}
    public struct Int64 {}
    public struct UInt64 {}
    public struct IntPtr {}
    public struct UIntPtr {}
    public struct Single {}
    public struct Decimal {}
    public struct Double {}
}

namespace System.Collections
{
    public interface IEnumerable {}
    public interface ICollection : IEnumerable {}
    public interface IList : ICollection {}
}

namespace System.Collections.Generic
{
    public interface IEnumerable<T> : System.Collections.IEnumerable {}
    public interface ICollection<T> : IEnumerable<T>, System.Collections.ICollection {}
    public interface IList<T> : ICollection<T>, System.Collections.IList {}
}";

                Project roslynProject = GetRoslynProject();

                SyntaxTree[] syntaxTrees = {SyntaxFactory.ParseSyntaxTree(mscorlibStub)};
                Compilation compilation = CSharpCompilation.Create(roslynProject.AssemblyName, syntaxTrees);
                Interlocked.CompareExchange(ref _fallbackCompilation, compilation, null);

                // Sanity check
                Debug.Assert(_fallbackCompilation.ObjectType.TypeKind == TypeKind.Class,
                    "Object type must be available");
            }

            return _fallbackCompilation;
        }

        public void Dispose() {
            if (!IsDisposed) {
                IsDisposed = true;
                Dispose(true);
            }

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            _roslynWorkspace.WorkspaceChanged -= OnWorkspaceChanged;

#if MAYBE_LATER
            this.HostProject.ReferencesChanged -= this.OnProjectReferencesChanged;
#endif

#if MAYBE_LATER
            this.hostFileChangeWatcher.FileChanged -= this.HostFileChangeWatcher_FileChanged;
            this.hostFileChangeWatcher.Dispose();
#endif

            KnownUIContexts.SolutionExistsAndFullyLoadedContext.PropertyChanged -=
                this.OnSolutionExistsAndFullyLoadedContext;
            KnownUIContexts.DebuggingContext.PropertyChanged -= this.OnDebuggingContextChanged;
            KnownUIContexts.SolutionBuildingContext.PropertyChanged -= this.OnSolutionBuildingContextChanged;

            lock (_notifyTimer) {
                _notifyTimer.Stop();
                _notifyTimer.Dispose();
                _notifyTimer.Elapsed -= this.OnNotifyTimer;
            }
        }
    }
}
