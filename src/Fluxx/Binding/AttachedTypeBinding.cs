using Fluxx.Api;

namespace Fluxx.Binding
{
    public abstract class AttachedTypeBinding : TypeBinding
    {
        protected AttachedTypeBinding(QualifiableName typeName) : base(typeName, TypeFlags.None) {}
    }
}
