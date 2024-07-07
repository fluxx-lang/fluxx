using System;
using System.Reflection;

namespace Faml.DotNet {
    public sealed class LoadedAssembly {
        private readonly Assembly _assembly;
        private readonly string? _assemblyQualifier;


        public LoadedAssembly(Assembly assembly) : this(assembly, null) {
        }

        public LoadedAssembly(Assembly assembly, string? assemblyQualifier) {
            this._assembly = assembly;
            this._assemblyQualifier = assemblyQualifier;
        }

        public Assembly Assembly => this._assembly;

        public override string ToString() {
            if (this._assemblyQualifier != null)
                return this._assemblyQualifier;
            else return this._assembly.FullName;
        }

        public Type? GetType(string typeName) {
            if (this._assemblyQualifier != null)
                return Type.GetType(typeName + ", " + this._assemblyQualifier);

            return this._assembly.GetType(typeName);
        }
    }
}
