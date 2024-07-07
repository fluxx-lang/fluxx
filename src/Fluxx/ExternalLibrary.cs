using Fluxx.Api;
using Fluxx.Binding;

namespace Fluxx
{
    public abstract class ExternalLibrary
    {
        public abstract TypeBinding? ResolveTypeBinding(QualifiableName className);

        public abstract AttachedTypeBinding? ResolveAttachedTypeBinding(QualifiableName className);
    }
}
