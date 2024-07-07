using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Fluxx.Api;
using Fluxx.Binding;
using Fluxx.Binding.External;
using Fluxx.Syntax;
using TypeTooling;
using TypeTooling.DotNet.RawTypes;
using TypeTooling.DotNet.RawTypes.Reflection;
using TypeTooling.RawTypes;
using TypeTooling.Types;

namespace Fluxx.DotNet
{
    public class DotNetProjectInfo
    {
        private readonly FamlProject project;
        private DotNetRawTypeProvider rawTypeProvider;
        private readonly List<LoadedAssembly> dependencyAssemblies = new List<LoadedAssembly>();

        public DotNetProjectInfo(FamlProject project)
        {
            this.project = project;
        }

        public DotNetRawTypeProvider RawTypeProvider
        {
            get => this.rawTypeProvider;
            set => this.rawTypeProvider = value;
        }

#if false
        /// <summary>
        /// Load the Faml.Types assembly, which is always present.
        /// </summary>
        public void LoadStandardDependencies() {
            if (_rawTypeProvider != null)
                return;

            if (_assemblyLoader == null)
                throw new Exception("No AssemblyLoader set on workspace");

            Assembly assembly = Assembly.Load("Faml.Types");
            if (assembly == null)
                throw new Exception("Could not load Faml.Types assembly");

            _dependencyAssemblies.Add(new LoadedAssembly(assembly));
        }
#endif

        /// <summary>
        /// We might add back the SDK concept later, for more flexible build time configuration, but
        /// don't use it currently.
        /// </summary>
        public void InitSdk()
        {
            IReadOnlyList<DotNetRawType> sdkTypes = this.rawTypeProvider.GetAssemblyAttributeReferencedTypes(typeof(FamlSdkAttribute).FullName);

            if (sdkTypes.Count == 0)
            {
                this.project.AddProjectError($"No FAML SDK specified. Check that your NuGet dependencies include a FAML SDK (with a FamlSdk assembly attribute).");
                return;
            }
            else if (sdkTypes.Count > 1)
            {
                this.project.AddProjectError($"Multiple FAML SDKs specified. Check that your NuGet dependencies include only a single that's a FAML SDK (with a FamlSdk assembly attribute)");
                return;
            }

            DotNetRawType famlSdkType = sdkTypes[0];

            try
            {
                object sdk = this.rawTypeProvider.Instantiate(famlSdkType);

                if (!(sdk is FamlSdk famlSdk))
                {
                    this.project.AddProjectError(
                        $"[FamlSdk] attribute specifies type {famlSdkType.FullName} which doesn't derive from {typeof(FamlSdk).FullName}");
                    return;
                }

                famlSdk.Init(this.project);
            }
            catch (Exception e)
            {
                this.project.AddProjectError($"Error initializing FAML SDK: {e.Message}");
            }
        }

        public void AddDependencyAssembly(Assembly assembly)
        {
            this.dependencyAssemblies.Add(new LoadedAssembly(assembly));
        }

#if false
        public void LoadDependencies(ProjectTypes.Project langProject, SyntaxNode nodeForDiagnostics) {
            if (_rawTypeProvider != null)
                return;

            foreach (DotNetAssembly dotNetAssembly in langProject.Dependencies) {
                LoadedAssembly loadedAssembly = LoadAssembly(dotNetAssembly, nodeForDiagnostics);
                if (loadedAssembly != null) {
                    _dependencyAssemblies.Add(loadedAssembly);

                    // Assembly is null is the case of the Windows UWP assembly, where we use assembly qualified names to load OS types
                    if (loadedAssembly.Assembly != null) {
                        AddTypeToolingProviders(loadedAssembly.Assembly, nodeForDiagnostics);
                        AddTypeToolingEnhancers(loadedAssembly.Assembly, nodeForDiagnostics);
                    }
                }
            }
        }
            
        private LoadedAssembly? LoadAssembly(DotNetAssembly dotNetAssembly, SyntaxNode nodeForDiagnostics)
        {
            if (_assemblyLoader == null) {
                nodeForDiagnostics.AddError("No AssemblyLoader set on program");
                return null;
            }

            string libraryPath = dotNetAssembly.Path.Path;

            // If the libraryPath is relative, then turn it into an absolute path, assuming that it's relative to the project root directory
            if (!Path.IsPathRooted(libraryPath) && _project.SourceProvider is FileSourceProvider fileSourceProvider) {
                string rootDirectory = fileSourceProvider.RootPath;
                string newLibraryPath = Path.Combine(rootDirectory, libraryPath);

                dotNetAssembly = new DotNetAssembly {
                    Name = dotNetAssembly.Name,
                    Path = new FilePath(newLibraryPath)
                };
            }

            LoadedAssembly loadedAssembly = _assemblyLoader.Load(dotNetAssembly, out string errorMessage);

            if (loadedAssembly == null && errorMessage == null)
                errorMessage =
                    $"AssemblyLoader returned null when loading library {dotNetAssembly}, but it should have returned an errorMessage instead";

            if (loadedAssembly == null)
                nodeForDiagnostics.AddError(errorMessage);

            return loadedAssembly;
        }
#endif

        public RawType? FindCompanionType(RawType rawType)
        {
            if (rawType is DotNetRawType dotNetRawType)
            {
                string fullName = dotNetRawType.FullName;

                string companionFullName = fullName + "TypeTooling";

                DotNetRawType? companionRawType = this.FindRawType(companionFullName);
                if (companionRawType != null)
                {
                    return companionRawType;
                }
            }

            return null;
        }

        private DotNetRawType? FindRawType(string typeName)
        {
            if (this.rawTypeProvider != null)
            {
                return this.rawTypeProvider.GetType(typeName);
            }

            // TODO: Log warning if multiple defined, in dev environment (and possibly in app too)
            foreach (LoadedAssembly dependencyAssembly in this.dependencyAssemblies)
            {
                Type type = dependencyAssembly.GetType(typeName);
                if (type != null)
                {
                    return new ReflectionDotNetRawType(type);
                }
            }

            return null;
        }

        public TypeBindingResult ResolveTypeBinding(QualifiableName typeName)
        {
            string typeNameString = typeName.ToString();

            DotNetRawType? foundType = this.FindRawType(typeNameString);
            if (foundType == null)
            {
                return TypeBindingResult.NotFoundResult;
            }

            TypeToolingType typeToolingType = this.project.GetTypeToolingType(foundType);
            if (typeToolingType == null)
            {
                return TypeBindingResult.NotFoundResult;
            }

            return this.TypeToolingTypeToTypeBinding(typeToolingType);
        }

        private string GetMultipleDefinitionError(string typeNameString)
        {
            StringBuilder message = new StringBuilder($"Found type {typeNameString} in multiple assemblies:");

            foreach (LoadedAssembly depenedencyAssembly in this.dependencyAssemblies)
            {
                Type assemblyType = depenedencyAssembly.GetType(typeNameString);
                if (assemblyType != null)
                {
                    message.Append("\n");
                    message.Append(depenedencyAssembly);
                }
            }

            return message.ToString();
        }

        public DotNetRawType? GetTypeToolingRawType(string typeName)
        {
            DotNetRawType? foundType = this.FindRawType(typeName);

            // TODO: Fix up this hack to load from proper system assembly, eventually
            if (foundType == null && typeName.StartsWith("System."))
            {
                Type currentAssemblyType = Type.GetType(typeName);
                if (currentAssemblyType != null)
                {
                    foundType = new ReflectionDotNetRawType(currentAssemblyType);
                }
            }

            if (foundType == null)
            {
                return null;
            }

            return foundType;
        }

        private TypeBindingResult TypeToolingTypeToTypeBinding(TypeToolingType type)
        {
            switch (type)
            {
                case SequenceType sequenceType:
                    TypeBindingResult elementTypeBindingResult = this.TypeToolingTypeToTypeBinding(sequenceType.ElementType);
                    if (!(elementTypeBindingResult is TypeBindingResult.Success elementTypeBinding))
                    {
                        return elementTypeBindingResult;
                    }

                    return new TypeBindingResult.Success(new ExternalSequenceTypeBinding(this.project, sequenceType, elementTypeBinding.TypeBinding));
                case ObjectType objectType:
                    return new TypeBindingResult.Success(new ExternalObjectTypeBinding(this.project, objectType));
                default:
                    return new TypeBindingResult.Error($"TypeTooling type {type.GetType().FullName} is currently not supported");
            }
        }

        public AttachedTypeBinding? ResolveAttachedTypeBinding(QualifiableName typeName)
        {
            DotNetRawType? foundType = this.FindRawType(typeName.ToString());
            if (foundType == null)
            {
                return null;
            }

            AttachedType attachedType = this.project.GetTypeToolingAttachedType(foundType);
            if (attachedType == null)
            {
                return null;
            }

            return new ExternalAttachedTypeBinding(this.project, attachedType);
        }

        public void DiscoverTypeToolingProviders()
        {
            IReadOnlyList<DotNetRawType> typeToolingProviderTypes = this.rawTypeProvider.GetAssemblyAttributeReferencedTypes(typeof(TypeToolingProviderAttribute).FullName);

            foreach (DotNetRawType typeToolingProviderType in typeToolingProviderTypes)
            {
                try
                {
                    object typeToolingProviderObject = this.rawTypeProvider.Instantiate(typeToolingProviderType, this.project.TypeToolingEnvironment);

                    if (!(typeToolingProviderObject is TypeToolingProvider typeToolingProvider))
                    {
                        this.project.AddProjectError(
                            $"[TypeToolingProvider] attribute specified type {typeToolingProviderType.FullName} should derive from {typeof(TypeToolingProvider).FullName} but doesn't");
                        continue;
                    }

                    this.project.AddTypeToolingProvider(typeToolingProvider);
                }
                catch (Exception e)
                {
                    this.project.AddProjectError($"Error instantiating TypeTooling provider {typeToolingProviderType}: {e.Message}");
                }
            }
        }

        private void AddTypeToolingProviders(Assembly loadedAssembly, SyntaxNode nodeForDiagnostics)
        {
            // Don't try to load type tooling providers for reflection only assemblies
            if (loadedAssembly.ReflectionOnly)
            {
                return;
            }

            foreach (Attribute attribute in loadedAssembly.GetCustomAttributes())
            {
                if (!(attribute is TypeToolingProviderAttribute providerAttribute))
                {
                    continue;
                }

                Type providerType = providerAttribute.ProviderType;

                ConstructorInfo constructorInfo = providerType.GetConstructor(new[] { typeof(TypeToolingEnvironment) });

                if (constructorInfo == null)
                {
                    nodeForDiagnostics.AddError(
                        $"The {providerType.FullName} class doesn't have a public constructor taking a single argument of type TypeToolingEnvironment");
                    continue;
                }

                object typeToolingProviderObject = constructorInfo.Invoke(new[] {this.project.TypeToolingEnvironment});

                if (!(typeToolingProviderObject is TypeToolingProvider typeToolingProvider))
                {
                    nodeForDiagnostics.AddError(
                        $"Assembly {loadedAssembly.FullName} [TypeToolingProvider] attribute specifies type {typeToolingProviderObject.GetType().FullName}, which doesn't implement TypeToolingProvider");
                    continue;
                }

                this.project.AddTypeToolingProvider(typeToolingProvider);
            }
        }

        private void AddTypeToolingEnhancers(Assembly loadedAssembly, SyntaxNode nodeForDiagnostics)
        {
            // Don't try to load type tooling providers for reflection only assemblies
            if (loadedAssembly.ReflectionOnly)
            {
                return;
            }

            foreach (Attribute attribute in loadedAssembly.GetCustomAttributes())
            {
                if (!(attribute is TypeToolingEnhancerAttribute enhancerAttribute))
                {
                    continue;
                }

                Type enhancerType = enhancerAttribute.EnhancerType;

                ConstructorInfo constructorInfo = enhancerType.GetConstructor(new[] { typeof(TypeToolingEnvironment) });

                if (constructorInfo == null)
                {
                    nodeForDiagnostics.AddError(
                        $"The {enhancerType.FullName} class doesn't have a public constructor taking a single argument of type TypeToolingEnvironment");
                    continue;
                }

                object typeToolingEnhancerObject = constructorInfo.Invoke(new[] { this.project.TypeToolingEnvironment });

                if (!(typeToolingEnhancerObject is TypeToolingEnhancer typeToolingEnhancer))
                {
                    nodeForDiagnostics.AddError(
                        $"Assembly {loadedAssembly.FullName} [TypeToolingEnhancer] attribute specifies type {typeToolingEnhancerObject.GetType().FullName}, which doesn't implement TypeToolingEnhancer");
                    continue;
                }

                this.project.AddDefaultTypeToolingEnhancer(typeToolingEnhancer);
            }
        }
    }
}
