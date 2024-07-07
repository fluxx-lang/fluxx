using System;
using System.Reflection;
using Fluxx.Api;
using Fluxx.Binding;
using Fluxx.Binding.External;
using TypeTooling.DotNet.RawTypes.Reflection;
using TypeTooling.Types;

namespace Fluxx.DotNet
{
    public sealed class DotNetExternalLibrary : ExternalLibrary
    {
        private readonly FamlProject project;
        private readonly Assembly assembly;

        public DotNetExternalLibrary(FamlProject project, Assembly assembly)
        {
            this.project = project;
            this.assembly = assembly;
        }

        public DotNetExternalLibrary(FamlProject project, string name)
        {
            this.project = project;

            var assemblyName = new AssemblyName(name);
            this.assembly = Assembly.Load(assemblyName);
        }

        public Assembly Assembly => this.assembly;

        public override TypeBinding? ResolveTypeBinding(QualifiableName className)
        {
            Type classType = this.assembly.GetType(className.ToString());
            if (classType == null)
            {
                return null;
            }

            TypeToolingType typeToolingType = this.project.GetTypeToolingType(new ReflectionDotNetRawType(classType));
            if (typeToolingType == null)
            {
                return null;
            }

            // TODO: Handle non-object types here too
            return new ExternalObjectTypeBinding(this.project, (ObjectType)typeToolingType);
        }

        public override AttachedTypeBinding? ResolveAttachedTypeBinding(QualifiableName className)
        {
            Type classType = this.assembly.GetType(className.ToString());
            if (classType == null)
            {
                return null;
            }

            AttachedType attachedType = this.project.GetTypeToolingAttachedType(new ReflectionDotNetRawType(classType));
            if (attachedType == null)
            {
                return null;
            }

            return new ExternalAttachedTypeBinding(this.project, attachedType);
        }
    }
}
