using System;
using System.Reflection;

namespace Faml.DotNet {
    public sealed class LoadedAssembly {
        private readonly Assembly _assembly;
        private readonly string? _assemblyQualifier;


        public LoadedAssembly(Assembly assembly) : this(assembly, null) {
        }

        public LoadedAssembly(Assembly assembly, string? assemblyQualifier) {
            _assembly = assembly;
            _assemblyQualifier = assemblyQualifier;
        }

        public Assembly Assembly => _assembly;

        public override string ToString() {
            if (_assemblyQualifier != null)
                return _assemblyQualifier;
            else return _assembly.FullName;
        }

        public Type? GetType(string typeName) {
            if (_assemblyQualifier != null)
                return Type.GetType(typeName + ", " + _assemblyQualifier);

            return _assembly.GetType(typeName);
        }
    }
}
