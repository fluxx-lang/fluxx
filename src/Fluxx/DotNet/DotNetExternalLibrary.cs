using System;
using System.Reflection;
using Faml.Api;
using Faml.Binding;
using Faml.Binding.External;
using TypeTooling.DotNet.RawTypes.Reflection;
using TypeTooling.Types;

namespace Faml.DotNet
{
    public sealed class DotNetExternalLibrary : ExternalLibrary
    {
        private readonly FamlProject _project;
        private readonly Assembly _assembly;


        public DotNetExternalLibrary(FamlProject project, Assembly assembly)
        {
            this._project = project;
            this._assembly = assembly;
        }

        public DotNetExternalLibrary(FamlProject project, string name)
        {
            this._project = project;

            var assemblyName = new AssemblyName(name);
            this._assembly = Assembly.Load(assemblyName);
        }

        public Assembly Assembly => this._assembly;

        public override TypeBinding? ResolveTypeBinding(QualifiableName className)
        {
            Type classType = this._assembly.GetType(className.ToString());
            if (classType == null)
            {
                return null;
            }

            TypeToolingType typeToolingType = this._project.GetTypeToolingType(new ReflectionDotNetRawType(classType));
            if (typeToolingType == null)
            {
                return null;
            }

            // TODO: Handle non-object types here too
            return new ExternalObjectTypeBinding(this._project, (ObjectType)typeToolingType);
        }

        public override AttachedTypeBinding? ResolveAttachedTypeBinding(QualifiableName className)
        {
            Type classType = this._assembly.GetType(className.ToString());
            if (classType == null)
            {
                return null;
            }

            AttachedType attachedType = this._project.GetTypeToolingAttachedType(new ReflectionDotNetRawType(classType));
            if (attachedType == null)
            {
                return null;
            }

            return new ExternalAttachedTypeBinding(this._project, attachedType);
        }
    }
}
