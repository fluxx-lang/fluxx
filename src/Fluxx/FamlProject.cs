using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Faml.Api;
using Faml.Binding;
using Faml.Binding.External;
using Faml.Binding.Resolver;
using Faml.CodeGeneration;
using Faml.DotNet;
using Faml.Interpreter;
using Faml.Lang;
using Faml.Parser;
using Faml.SourceProviders;
using Faml.Syntax;
using Faml.Syntax.Expression;
using Microsoft.CodeAnalysis.Text;
using TypeTooling;
using TypeTooling.DotNet;
using TypeTooling.DotNet.CodeGeneration;
using TypeTooling.DotNet.RawTypes;
using TypeTooling.DotNet.RawTypes.Reflection;
using TypeTooling.RawTypes;
using TypeTooling.Types;
using TypeTooling.Visualize;
using Diagnostic = Faml.CodeAnalysis.Diagnostic;

namespace Faml {
    public class FamlProject {
        private readonly FamlWorkspace _workspace;
        private readonly SourceProvider _sourceProvider;
        private readonly Dictionary<string, string> _sourceOverrides = new Dictionary<string, string>();
        private bool _fullyInitialized;

        private readonly FamlTypeToolingEnvironment _typeToolingEnvironment;
        private readonly List<TypeToolingProvider> _defaultTypeToolingProviders = new List<TypeToolingProvider>();
        private readonly List<TypeToolingEnhancer> _defaultTypeToolingEnhancers = new List<TypeToolingEnhancer>();
        private readonly List<TypeToolingProvider> _typeToolingProviders = new List<TypeToolingProvider>();
        private readonly List<TypeToolingEnhancer> _typeToolingEnhancers = new List<TypeToolingEnhancer>();
        private readonly ConcurrentDictionary<RawType, TypeToolingType> _cachedTypeToolingTypes = new ConcurrentDictionary<RawType, TypeToolingType>();
        private readonly ConcurrentDictionary<string, RawType> _cachedTypeToolingTypeNamesToRawTypes = new ConcurrentDictionary<string, RawType>();
        private readonly ConcurrentDictionary<RawType, AttachedType> _cachedTypeToolingAttachedTypes = new ConcurrentDictionary<RawType, AttachedType>();
        private ExternalObjectTypeBinding? _exampleTypeBinding;
        private ExternalObjectTypeBinding? _examplesTypeBinding;

        private readonly Dictionary<QualifiableName, FamlModule> _modules = new Dictionary<QualifiableName, FamlModule>();
        private readonly ConcurrentDictionary<ModuleSyntax, ModuleEvals> _modulesEvals = new ConcurrentDictionary<ModuleSyntax, ModuleEvals>();
        private readonly ConcurrentDictionary<ModuleSyntax, ModuleDelegates> _moduleDelegates = new ConcurrentDictionary<ModuleSyntax, ModuleDelegates>();

        private readonly CaseStyle _caseStyle;

        // .NET specific platform implementation stuff is kept here
        private readonly DotNetProjectInfo _dotNetProjectInfo;
        private AppProjectInfo _appProjectInfo;    // Set on app projects; null on library projects

        private readonly List<Diagnostic> _projectDiagnostics = new List<CodeAnalysis.Diagnostic>();
        private readonly Dictionary<QualifiableName, List<Diagnostic>> _modulesDiagnostics =
            new Dictionary<QualifiableName, List<Diagnostic>>();

        public FamlProject(FamlWorkspace workspace, SourceProvider sourceProvider) {
            this._workspace = workspace;
            this._sourceProvider = sourceProvider;
            this._dotNetProjectInfo = new DotNetProjectInfo(this);

            this._typeToolingEnvironment = new FamlTypeToolingEnvironment(this);

            //_defaultTypeToolingProviders.Add(new XamlTypeToolingProvider(_typeToolingEnvironment));
            this._defaultTypeToolingProviders.Add(new DotNetTypeToolingProvider(this._typeToolingEnvironment));
        }

        public void FullyInitialize() {
            if (this._fullyInitialized)
                throw new Exception("Project already initialized");

#if false
            // Faml.Types is always present, by default
            _dotNetProjectInfo.LoadStandardDependencies();
#endif
#if MAYBE_LATER
            _dotNetProjectInfo.InitSdk();
#endif

#if LATER_MAYBE_SUPPORT_PROJECT
            string? projectFileSource = GetSource("project.faml");
            if (projectFileSource != null)
                UpdateModule(new QualifiableName("project"), projectFileSource, false);
#endif

#if LATER
            _exampleTypeBinding = ResolveStandardType(typeof(ExampleResult));
            _examplesTypeBinding = ResolveStandardType(typeof(ExamplesResult));
#endif

            this._dotNetProjectInfo.DiscoverTypeToolingProviders();

            this._fullyInitialized = true;
        }

        public bool FullyInitialized => this._fullyInitialized;

        public bool TypeProviderReady() {
            return this._dotNetProjectInfo.RawTypeProvider?.IsReady ?? false;
        }

        private ExternalObjectTypeBinding? ResolveStandardType(Type type) {
            TypeBindingResult typeBindingResult = this.ResolveTypeBinding(new QualifiableName(type.FullName));
            if (typeBindingResult is TypeBindingResult.Success success)
                return (ExternalObjectTypeBinding) success.TypeBinding;
            else {
                this.AddProjectError(
                    typeBindingResult.GetNotFoundOrOtherErrorMessage(
                        $"Unexpectedly, could not resolve {type.FullName} type binding from Faml.Types"));
                return null;
            }
        }

        public FamlWorkspace Workspace => this._workspace;

        public SourceProvider SourceProvider => this._sourceProvider;

        public void SetSource(string path, string? source) {
            if (source == null)
                this._sourceOverrides.Remove(path);
            else this._sourceOverrides[path] = source;
        }

        public string? GetSource(string path) {
            if (this._sourceOverrides.TryGetValue(path, out string source))
                return source;

            return this._sourceProvider.GetTextResource(path);
        }

        public string GetModuleFilePath(QualifiableName moduleName) {
            string moduleRelativePath = moduleName.ToString().Replace('.', Path.PathSeparator) + ".faml";
            return this._sourceProvider.RootPath + Path.PathSeparator + moduleRelativePath;
        }

        private void UpdateModule(QualifiableName moduleName, string moduleSource, bool isStandaloneModule) {
            FamlModule module = SourceParser.ParseModule(this, moduleName, SourceText.From(moduleSource));

            ModuleSyntax moduleSyntax = module.ModuleSyntax;

            FunctionInvocationSyntax? projectDefinition = moduleSyntax.ProjectDefinition;
            if (projectDefinition != null) {
                if (isStandaloneModule || moduleName.ToString() == "project")
                    this.ProcessProject(moduleName, projectDefinition);
                else projectDefinition.AddError("'App' function can only be specified in the main module or in 'project.faml'");
            }

            if (this.TypeProviderReady())
                moduleSyntax.ResolveAllBindings();

            this._modules[moduleName] = module;
        }

        public TypeToolingEnvironment TypeToolingEnvironment => this._typeToolingEnvironment;

        public void AddTypeToolingProvider(TypeToolingProvider typeToolingProvider) {
            this._typeToolingProviders.Add(typeToolingProvider);
        }

        public void AddDefaultTypeToolingProvider(TypeToolingProvider typeToolingProvider) {
            this._defaultTypeToolingProviders.Add(typeToolingProvider);
        }

        public void AddDefaultTypeToolingEnhancer(TypeToolingEnhancer typeToolingEnhancer) {
            this._defaultTypeToolingEnhancers.Add(typeToolingEnhancer);
        }

        public TypeToolingType? GetTypeToolingType(RawType rawType) {
            return this._cachedTypeToolingTypes.GetOrAdd(rawType, this.DoGetTypeToolingType);
        }

        public void ExternalDependenciesChanged() {
            this._cachedTypeToolingTypes.Clear();
            this._cachedTypeToolingTypeNamesToRawTypes.Clear();
            this._cachedTypeToolingAttachedTypes.Clear();

            if (!this._fullyInitialized) {
                if (this._dotNetProjectInfo.RawTypeProvider != null && this._dotNetProjectInfo.RawTypeProvider.IsReady)
                    this.FullyInitialize();
            }
        }

        private TypeToolingType? DoGetTypeToolingType(RawType rawType) {
            RawType? companionType = this._dotNetProjectInfo.FindCompanionType(rawType);

            foreach (TypeToolingProvider provider in this._typeToolingProviders) {
                TypeToolingType? typeToolingType = provider.ProvideType(rawType, companionType);

                if (typeToolingType != null)
                    return this.EnhanceTypeToolingType(typeToolingType);
            }

            // Check the default providers last
            foreach (TypeToolingProvider defaultProvider in this._defaultTypeToolingProviders) {
                TypeToolingType? typeToolingType = defaultProvider.ProvideType(rawType, companionType);

                if (typeToolingType != null)
                    return this.EnhanceTypeToolingType(typeToolingType);
            }

            return null;
        }

        public RawType? GetTypeToolingRawType(string typeName) {
            return this._cachedTypeToolingTypeNamesToRawTypes.GetOrAdd(typeName, this.DoGetTypeToolingRawType);
        }

        private RawType? DoGetTypeToolingRawType(string typeName) {
            return this._dotNetProjectInfo.GetTypeToolingRawType(typeName);
        }

        public AttachedType? GetTypeToolingAttachedType(RawType rawType) {
            return this._cachedTypeToolingAttachedTypes.GetOrAdd(rawType, this.DoGetTypeToolingAttachedType);
        }

        public object Instantiate(RawType rawType, params object[] args) {
            if (rawType is DotNetRawType dotNetRawType)
                return this._dotNetProjectInfo.RawTypeProvider.Instantiate(dotNetRawType, args);
            else throw new UserViewableException($"Only DotNetRawType raw types can be instantiated; raw type {rawType.GetType().FullName} isn't supported");
        }

        private AttachedType? DoGetTypeToolingAttachedType(RawType rawType) {
            foreach (TypeToolingProvider provider in this._typeToolingProviders) {
                AttachedType? attachedType = provider.ProvideAttachedType(rawType, null);

                if (attachedType != null)
                    return this.EnhanceTypeToolingAttachedType(attachedType);
            }

            // Check the default providers last
            foreach (TypeToolingProvider defaultProvider in this._defaultTypeToolingProviders) {
                AttachedType? attachedType = defaultProvider.ProvideAttachedType(rawType, null);

                if (attachedType != null)
                    return this.EnhanceTypeToolingAttachedType(attachedType);
            }

            return null;
        }

        private TypeToolingType EnhanceTypeToolingType(TypeToolingType typeToolingType) {
            foreach (TypeToolingEnhancer enhancer in this._typeToolingEnhancers) {
                TypeToolingType enhancedType = enhancer.EnhanceType(typeToolingType);
                if (enhancedType != null)
                    typeToolingType = enhancedType;
            }

            // Check the default enhancers last
            foreach (TypeToolingEnhancer defaultEnhancer in this._defaultTypeToolingEnhancers) {
                TypeToolingType enhancedType = defaultEnhancer.EnhanceType(typeToolingType);
                if (enhancedType != null)
                    typeToolingType = enhancedType;
            }

            return typeToolingType;
        }

        private AttachedType EnhanceTypeToolingAttachedType(AttachedType attachedType) {
            foreach (TypeToolingEnhancer enhancer in this._typeToolingEnhancers) {
                AttachedType enhancedType = enhancer.EnhanceAttachedType(attachedType);
                if (enhancedType != null)
                    attachedType = enhancedType;
            }

            // Check the default enhancers last
            foreach (TypeToolingEnhancer defaultEnhancer in this._defaultTypeToolingEnhancers) {
                AttachedType enhancedType = defaultEnhancer.EnhanceAttachedType(attachedType);
                if (enhancedType != null)
                    attachedType = enhancedType;
            }

            return attachedType;
        }

        private void ProcessProject(QualifiableName projectModuleName, FunctionInvocationSyntax projectDefinition) {
            var projectDefinitionBindingResolver = new ProjectDefinitionBindingResolver(this);

            projectDefinition.VisitNodeAndDescendentsPostorder((syntaxNode) => { syntaxNode.ResolveExplicitTypeBindings(projectDefinitionBindingResolver); });

            // Now resolve all the bindings.  Do it postorder, bottom up, so a node can assume its descendents' bindings have
            // already been resolved when trying to resolve its own bindings.
            projectDefinition.VisitNodeAndDescendentsPostorder((syntaxNode) => { syntaxNode.ResolveBindings(projectDefinitionBindingResolver); });

            // If the project node has any errors in it, give up - don't try to interpret it
            if (this.AnyErrorsInModuleSourceSpan(projectDefinition.GetModuleSyntax(), projectDefinition.Span))
                return;

            var projectFunctionEval = (ObjectEval) new CreateEvals(this.TypeToolingEnvironment).CreateExpressionEval(projectDefinition);

            Context.Reset();
            var projectObject = (ProjectTypes.Project) projectFunctionEval.Eval();

            if (projectObject is ProjectTypes.App app) {
                string developmentMachine = app.DevelopmentMachine;
                this._appProjectInfo = new AppProjectInfo {
                    DevelopmentMachine = developmentMachine
                };
            }

#if false
            _dotNetProjectInfo.LoadDependencies(projectObject, projectDefinition);
#endif
        }

        public TypeBindingResult ResolveTypeBinding(QualifiableName typeName) {
            return this._dotNetProjectInfo.ResolveTypeBinding(typeName);
        }

        public AttachedTypeBinding? ResolveAttachedTypeBinding(QualifiableName typeName) {
            return this._dotNetProjectInfo.ResolveAttachedTypeBinding(typeName);
        }

        public ModuleEvals GetModuleEvals(ModuleSyntax moduleSyntax) {
            return this._modulesEvals.GetOrAdd(moduleSyntax, syntax => new ModuleEvals());
        }

        public ModuleDelegates GetModuleDelegates(ModuleSyntax moduleSyntax) {
            return this._moduleDelegates.GetOrAdd(moduleSyntax, syntax => new ModuleDelegates(this.TypeToolingEnvironment));
        }

        public DotNetProjectInfo DotNetProjectInfo => this._dotNetProjectInfo;

        public AppProjectInfo AppProjectInfo => this._appProjectInfo;

        public IEnumerable<Diagnostic> ProjectDiagnostics => this._projectDiagnostics;

        public IEnumerable<Diagnostic> GetAllDiagnostics() {
            foreach (Diagnostic diagnostic in this._projectDiagnostics)
                yield return diagnostic;

            foreach (KeyValuePair<QualifiableName, List<CodeAnalysis.Diagnostic>> moduleDiagnosticsPair in this._modulesDiagnostics) {
                foreach (Diagnostic diagnostic in moduleDiagnosticsPair.Value)
                    yield return diagnostic;
            }
        }

        public IEnumerable<Diagnostic> GetModuleDiagnostics(QualifiableName moduleName) {
            if (! this._modulesDiagnostics.TryGetValue(moduleName, out List<CodeAnalysis.Diagnostic> moduleDiagnostics))
                return Enumerable.Empty<CodeAnalysis.Diagnostic>();
            return moduleDiagnostics;
        }

        public bool AnyErrorsInModuleSourceSpan(ModuleSyntax moduleSyntax, TextSpan span) {
            QualifiableName moduleName = moduleSyntax.ModuleName;

            if (!this._modulesDiagnostics.TryGetValue(moduleName, out List<Diagnostic> diagnostics))
                return false;

            foreach (Diagnostic diagnostic in diagnostics) {
                if (diagnostic.Severity == Api.DiagnosticSeverity.Error && diagnostic.SourceSpan.OverlapsWith(span))
                    return true;
            }

            return false;
        }

        // TODO: Update this to only check for errors
        public bool AnyErrors => this._projectDiagnostics.Count > 0 || this._modulesDiagnostics.Count > 0;

        public ExternalObjectTypeBinding? ExampleTypeBinding => this._exampleTypeBinding;

        public ExternalObjectTypeBinding? ExamplesTypeBinding => this._examplesTypeBinding;

        public void AddModuleDiagnostic(QualifiableName moduleName, CodeAnalysis.Diagnostic diagnostic) {
            if (!this._modulesDiagnostics.TryGetValue(moduleName, out List<Diagnostic> diagnostics)) {
                diagnostics = new List<CodeAnalysis.Diagnostic>();
                this._modulesDiagnostics.Add(moduleName, diagnostics);
            }
            diagnostics.Add(diagnostic);
        }

        public void AddProjectDiagnostic(CodeAnalysis.Diagnostic diagnostic) {
            this._projectDiagnostics.Add(diagnostic);
        }

        public void AddProjectError(string message) {
            this.AddProjectDiagnostic(new Diagnostic(Api.DiagnosticSeverity.Error, message));
        }

        public FunctionDefinitionSyntax? GetFunctionWithName(QualifiableName name) {
            if (! name.IsQualified())
                throw new UserViewableException($"The function name '{name}' should be fully qualified (contain at least one period)");
            return this.GetModuleIfExists(name.GetQualifier())?.ModuleSyntax.GetFunctionDefinition(name.GetLastComponent());
        }

        public Delegate CreateFunctionInvocationDelegate(QualifiableName functionName, Args args) {
            FunctionDefinitionSyntax? function = this.GetFunctionWithName(functionName);
            if (function == null)
                throw new UserViewableException($"Function {functionName} not found");

            FunctionDelegateHolder delegateHolder = function.GetModule().ModuleDelegates.GetOrCreateFunctionDelegate(function);

            if (args.Count != 0)
                throw new UserViewableException("Currently, only zero-argument functions are supported for CreateFunctionInvocationDelegate");

            Delegate? functionDelegate = delegateHolder.FunctionDelegate;
            if (functionDelegate == null)
                throw new UserViewableException($"FunctionDelegateHolder doesn't contain a delegate for function {functionName}");

            return functionDelegate;
        }

        public Eval CreateFunctionInvocationEval(QualifiableName functionName, Args args) {
            FunctionDefinitionSyntax? function = this.GetFunctionWithName(functionName);
            if (function == null)
                throw new UserViewableException($"Function {functionName} not found");
            return new CreateEvals(this.TypeToolingEnvironment).CreateFunctionInvocationEval(function, args);
        }

        public Eval GetOrCreateFunctionDefinitionEval(FunctionDefinitionSyntax functionDefinition) {
            return new CreateEvals(this.TypeToolingEnvironment).GetOrCreateFunctionEval(functionDefinition);
        }

        public ExampleResult[] EvaluateExample(QualifiableName moduleName, int exampleIndex) {
            FamlModule? module = this.GetModuleIfExists(moduleName);
            if (module == null)
                throw new UserViewableException($"Module '{moduleName.ToString()}' not found");

            ExampleDefinitionSyntax exampleDefinition = module.ModuleSyntax.GetExampleDefinitionAtIndex(exampleIndex);
            return this.EvaluateExample(exampleDefinition);
        }

        public ExampleResult[] EvaluateExample(ExampleDefinitionSyntax exampleDefinition) {
            try {
                var exampleResults = new List<ExampleResult>();

                ObjectEval exampleEval = new CreateEvals(this.TypeToolingEnvironment).GetOrCreateExampleEval(exampleDefinition);
                object exampleValue = exampleEval.Eval();

                if (exampleValue is ExamplesResult examplesResult) {
                    foreach (ExampleResult exampleResult in examplesResult.Content)
                        exampleResults.Add(exampleResult);
                }
                else if (exampleValue is IEnumerable exampleResultsEnumerable) {
                    foreach (object exampleResultObject in exampleResultsEnumerable) {
                        if (exampleResultObject is ExampleResult exampleResult)
                            exampleResults.Add(exampleResult);
                        else
                            exampleResults.Add(new ExampleResult {
                                Content = exampleValue
                            });
                    }
                }
                else if (exampleValue is ExampleResult exampleResult) {
                    exampleResults.Add(this.VisualizeExampleResult(exampleResult));
                }
                else {
                    exampleResults.Add(new ExampleResult {
                        Content = exampleValue
                    });
                }

                return exampleResults.Select(this.VisualizeExampleResult).ToArray();
            }
            catch (Exception e) {
                return new[] {
                    new ExampleResult {
                        Content = new VisualizableError(e.Message)
                    }
                };
            }
        }

        private ExampleResult VisualizeExampleResult(ExampleResult exampleResult) {
            // If there's a TypeToolingType for this type, and it supports visualizion, then convert the object to a visualized version
            Type contentType = exampleResult.Content.GetType();
            TypeToolingType contentTypeToolingType = this.GetTypeToolingType(new ReflectionDotNetRawType(contentType));
            object? visualizedObject = contentTypeToolingType?.GetVisualizer()?.Visualize(exampleResult.Content);

            if (visualizedObject != null)
                exampleResult.Content = visualizedObject;

            return exampleResult;
        }

        public FamlModule? GetModuleIfExists(QualifiableName moduleName) {
            this._modules.TryGetValue(moduleName, out FamlModule module);
            return module;
        }

        public FamlModule GetModule(QualifiableName moduleName) {
            FamlModule? module = this.GetModuleIfExists(moduleName);
            if (module == null)
                throw new UserViewableException($"Module '{moduleName}' not found");
            return module;
        }

        /*
        public object? renderExpression(string expressionSource) {
            string sourceText = "eval{} = " + expressionSource;

            ModuleName moduleName = new ModuleName("eval.faml");
            Module module = createModule(sourceText, moduleName, false);

            // TODO: Prevent errors from accumulating here
            if (this.anyErrors)
                return null;

            var functionDefinition = (FunctionDefinition) module.moduleItems[0];

            TypeBinding typeBinding = functionDefinition.returnTypeBinding;

            if (!(typeBinding is ObjectTypeBinding))
                throw new Exception("Only object expressions are currently supported here");

            CreateEvals createEvals = new CreateEvals(this);
            ModuleEvals moduleEvals = createEvals.createModuleEvals(module);

            Eval functionEval = moduleEvals.getFunctionEval(functionDefinition);

            // TODO: Support other (primitive) return types
            var value = ((ObjectEval) functionEval).eval();

            return renderObject(value, typeBinding);
        }
        */

        /*
        public ModuleSyntax LoadMainModule(ModuleName modulePath) {
            return LoadModule(modulePath, true);
        }

        private ModuleSyntax LoadModule(ModuleName modulePath, bool isMainModule) {
            string path = modulePath.Name;
            string source = GetSource(path);
            if (source == null)
                throw new Exception($"Resource {path} not found");

            return LoadModuleFromSource(modulePath, source, isMainModule);
        }
        */

        public void UpdateSource(string path, string updatedSource) {
            QualifiableName moduleName = QualifiableName.ModuleNameFromRelativePath(path);

            this._modules.Remove(moduleName);
            this._modulesEvals.Clear();

            // TODO: Fix this up - or just remove everything
            this._modulesDiagnostics.Remove(moduleName);

            this._sourceOverrides[path] = updatedSource;

            this.UpdateModule(moduleName, updatedSource, false);
        }

        public FamlModule? GetLoadedModule(QualifiableName moduleName) {
            return !this._modules.TryGetValue(moduleName, out FamlModule syntaxTree) ? null : syntaxTree;
        }

        public IEnumerable<FamlModule> Modules => this._modules.Values;
    }
}
