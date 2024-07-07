using Faml.Api;

namespace Faml.Binding
{
    public abstract class FunctionTypeBinding : TypeBinding
    {
        protected FunctionTypeBinding(QualifiableName typeName) : base(typeName, TypeFlags.None) {}
    }
}
