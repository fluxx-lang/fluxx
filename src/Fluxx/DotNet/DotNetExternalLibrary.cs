using System;
using System.Reflection;
using Faml.Api;
using Faml.Binding;
using Faml.Binding.External;
using TypeTooling.DotNet.RawTypes.Reflection;
using TypeTooling.Types;

namespace Faml.DotNet {
    public sealed class DotNetExternalLibrary : ExternalLibrary {
        private readonly FamlProject _project;
        private readonly Assembly _assembly;


        public DotNetExternalLibrary(FamlProject project, Assembly assembly) {
            _project = project;
            _assembly = assembly;
        }

        public DotNetExternalLibrary(FamlProject project, string name) {
            _project = project;

            var assemblyName = new AssemblyName(name);
            _assembly = Assembly.Load(assemblyName);
        }

        public Assembly Assembly => _assembly;

        public override TypeBinding? ResolveTypeBinding(QualifiableName className) {
            Type classType = _assembly.GetType(className.ToString());
            if (classType == null)
                return null;

            TypeToolingType typeToolingType = _project.GetTypeToolingType(new ReflectionDotNetRawType(classType));
            if (typeToolingType == null)
                return null;

            // TODO: Handle non-object types here too
            return new ExternalObjectTypeBinding(_project, (ObjectType) typeToolingType);
        }

        public override AttachedTypeBinding? ResolveAttachedTypeBinding(QualifiableName className) {
            Type classType = _assembly.GetType(className.ToString());
            if (classType == null)
                return null;

            AttachedType attachedType = _project.GetTypeToolingAttachedType(new ReflectionDotNetRawType(classType));
            if (attachedType == null)
                return null;

            return new ExternalAttachedTypeBinding(_project, attachedType);
        }
    }
}
