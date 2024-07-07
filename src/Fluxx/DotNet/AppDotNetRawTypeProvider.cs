using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TypeTooling.DotNet.RawTypes;
using TypeTooling.DotNet.RawTypes.Reflection;

namespace Faml.DotNet {
    /// <summary>
    /// This TypeProvider is used when running in the app, as opposed to at compile time. It searches
    /// all referenced assemblies (non recursive) for the specific type.
    /// </summary>
    public class AppDotNetRawTypeProvider : DotNetRawTypeProvider {
        private readonly Assembly _mainAssembly;
        private readonly Lazy<List<Assembly>> _assemblies;
        private string _windowsRuntimeAssemblyQualifier = null;

        public AppDotNetRawTypeProvider(Assembly mainAssembly) {
            this._mainAssembly = mainAssembly;
            this._assemblies = new Lazy<List<Assembly>>(this.LoadAssemblies);
        }

        public override bool IsReady => true;

#if false
        public override Assembly GetAssembly(string assemblySimpleName) {
            foreach (Assembly assembly in _assemblies.Value) {
                if (assembly.GetName().Name == assemblySimpleName)
                    return assembly;
            }

            return null;
        }
#endif

        private List<Assembly> LoadAssemblies() {
            var assemblies = new List<Assembly>();
            var assemblyNames = new HashSet<string>();

            this.AddAssemblies(this._mainAssembly, assemblies, assemblyNames);

            return assemblies;
        }

        private void AddAssemblies(Assembly assembly, List<Assembly> assemblies, HashSet<string> assemblyFullNames) {
            AssemblyName assemblyName = assembly.GetName();
            string assemblyFullName = assemblyName.FullName;

            // The loaded assembly can have a slightly different name than its reference, so check again if we've already
            // processed this assembly
            if (assemblyFullNames.Contains(assemblyFullName))
            {
                return;
            }

            // Skip system assemblies
            string assemblySimpleName = assemblyName.Name;
            if (assemblySimpleName == "System" || assemblySimpleName.StartsWith("System.") || assemblySimpleName.StartsWith("netstandard"))
            {
                return;
            }

            assemblies.Add(assembly);
            assemblyFullNames.Add(assemblyFullName);

            AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();
            foreach (AssemblyName referencedAssemblyName in referencedAssemblies) {
                // For Windows runtime assemblies (e.g. for UWP apps), don't try to load them - they should already be loaded & loading them
                // won't work anyway. Instead save off the assembly qualifier to use, when attempting to load types from the windows runtime
                // assembly.
                if (referencedAssemblyName.ContentType == AssemblyContentType.WindowsRuntime) {
                    this._windowsRuntimeAssemblyQualifier = "Windows, ContentType = WindowsRuntime";
                    continue;
                }

                if (!assemblyFullNames.Contains(referencedAssemblyName.FullName)) {
                    Assembly referencedAssembly = Assembly.Load(referencedAssemblyName);
                    this.AddAssemblies(referencedAssembly, assemblies, assemblyFullNames);
                }
            }
        }

        public override DotNetRawType? GetType(string typeName) {
            foreach (Assembly referencedAssembly in this._assemblies.Value) {
                Type type = referencedAssembly.GetType(typeName);
                if (type != null)
                {
                    return new ReflectionDotNetRawType(type);
                }
            }

            if (this._windowsRuntimeAssemblyQualifier != null) {
                Type type = Type.GetType(typeName + ", " + this._windowsRuntimeAssemblyQualifier);
                if (type != null)
                {
                    return new ReflectionDotNetRawType(type);
                }
            }

            return null;
        }

        public override IReadOnlyList<DotNetRawType> GetAssemblyAttributeReferencedTypes(string attributeFullName) {
            // TODO: Implement this
            return Array.Empty<DotNetRawType>();
        }

        public override Task<IEnumerable<DotNetRawType>> FindTypesAssignableToAsync(DotNetRawType rawType, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public override object Instantiate(DotNetRawType type, params object[] args) {
            Type reflectionType = ((ReflectionDotNetRawType) type).Type;
            return Activator.CreateInstance(reflectionType, args);
        }
    }
}
