using Faml.Api;
using Faml.Binding;

namespace Faml
{
    public abstract class ExternalLibrary
    {
        public abstract TypeBinding? ResolveTypeBinding(QualifiableName className);

        public abstract AttachedTypeBinding? ResolveAttachedTypeBinding(QualifiableName className);
    }
}
