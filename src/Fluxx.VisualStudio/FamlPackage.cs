using System;
using System.Runtime.InteropServices;
using System.Threading;
using EnvDTE80;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Task = System.Threading.Tasks.Task;

namespace Faml.VisualStudio {
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(FamlPackageGuidString)]
    public sealed class FamlPackage : AsyncPackage, IVsSolutionEvents
    {
        private DTE2 _dte2;
        private TaskManager _taskManager;
        private IComponentModel _componentModel;
        private ITextDocumentFactoryService _textDocumentFactoryService;
        private IVsSolution _vsSolution;
        private VisualStudioWorkspace _roslynWorkspace;
        private FamlVisualStudioWorkspace _famlVisualStudioWorkspace;

        public const string FamlPackageGuidString = "823f6734-9032-4404-b7f9-0afc775d9814";
        public const string FamlContentType = "faml";

        // TODO: Update this??
        private static FamlPackage _instance = null;
        public static FamlPackage Instance {
            get {
                ThreadHelper.ThrowIfNotOnUIThread();

                if (_instance == null) {
                    if (!(Package.GetGlobalService(typeof(SVsShell)) is IVsShell shell))
                        throw new InvalidOperationException("Error getting IVsShell");

                    var guid = new Guid(FamlPackageGuidString);
                    shell.LoadPackage(ref guid, out IVsPackage _);
                }

                return _instance;
            }
        }

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress) {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            _dte2 = GetService(typeof(SDTE)) as DTE2;

            _componentModel = (IComponentModel)this.GetService(typeof(SComponentModel));
            _textDocumentFactoryService = _componentModel.GetService<ITextDocumentFactoryService>();
            _vsSolution = (IVsSolution)GetService(typeof(SVsSolution));
            _roslynWorkspace = _componentModel.GetService<VisualStudioWorkspace>();

            _roslynWorkspace.WorkspaceChanged += delegate (object sender, WorkspaceChangeEventArgs args) {
                System.Diagnostics.Debug.WriteLine($"WorkspaceChanged; {args.Kind}");
            };

            _instance = this;

            _taskManager = new TaskManager(this);
            ConvertToFamlCommand.Initialize(this);

            base.Initialize();

            _vsSolution.AdviseSolutionEvents(this, out _);
        }

        public DTE2 Dte2 => _dte2;

        public TaskManager TaskManager => _taskManager;

        public IVsSolution VsSolution => _vsSolution;

        public VisualStudioWorkspace RoslynWorkspace => _roslynWorkspace;

        public FamlVisualStudioWorkspace GetOrCreateFamlWorkspace() {
            // The workspace is created on demand and goes away when the solution is closed
            if (_famlVisualStudioWorkspace == null)
                _famlVisualStudioWorkspace = new FamlVisualStudioWorkspace(_componentModel);

            return _famlVisualStudioWorkspace;
        }

        public ITextDocumentFactoryService TextDocumentFactoryService => _textDocumentFactoryService;

        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded) => VSConstants.S_OK;
        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel) => VSConstants.S_OK;
        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved) => VSConstants.S_OK;
        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy) => VSConstants.S_OK;
        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel) => VSConstants.S_OK;
        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy) => VSConstants.S_OK;
        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution) => VSConstants.S_OK;
        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel) => VSConstants.S_OK;
        public int OnBeforeCloseSolution(object pUnkReserved) => VSConstants.S_OK;

        public int OnAfterCloseSolution(object pUnkReserved) {
            _famlVisualStudioWorkspace = null;
            return VSConstants.S_OK;
        }
    }
}
