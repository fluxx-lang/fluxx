using Fluxx.Api;

namespace Fluxx.Binding
{
    public abstract class FunctionTypeBinding : TypeBinding
    {
        protected FunctionTypeBinding(QualifiableName typeName) : base(typeName, TypeFlags.None) {}
    }
}
