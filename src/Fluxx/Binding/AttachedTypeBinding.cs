using Faml.Api;

namespace Faml.Binding
{
    public abstract class AttachedTypeBinding : TypeBinding
    {
        protected AttachedTypeBinding(QualifiableName typeName) : base(typeName, TypeFlags.None) {}
    }
}
