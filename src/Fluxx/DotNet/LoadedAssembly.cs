using System;
using System.Reflection;

namespace Fluxx.DotNet
{
    public sealed class LoadedAssembly
    {
        private readonly Assembly assembly;
        private readonly string? assemblyQualifier;

        public LoadedAssembly(Assembly assembly) : this(assembly, null)
        {
        }

        public LoadedAssembly(Assembly assembly, string? assemblyQualifier)
        {
            this.assembly = assembly;
            this.assemblyQualifier = assemblyQualifier;
        }

        public Assembly Assembly => this.assembly;

        public override string ToString()
        {
            if (this.assemblyQualifier != null)
            {
                return this.assemblyQualifier;
            }
            else
            {
                return this.assembly.FullName;
            }
        }

        public Type? GetType(string typeName)
        {
            if (this.assemblyQualifier != null)
            {
                return Type.GetType(typeName + ", " + this.assemblyQualifier);
            }

            return this.assembly.GetType(typeName);
        }
    }
}
