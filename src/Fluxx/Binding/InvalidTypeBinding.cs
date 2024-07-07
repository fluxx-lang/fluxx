using Faml.Api;

namespace Faml.Binding
{
    public class InvalidTypeBinding : TypeBinding
    {
        public static InvalidTypeBinding Instance = new InvalidTypeBinding(new QualifiableName("InvalidType"));

        public InvalidTypeBinding(QualifiableName typeName) : base(typeName, TypeFlags.None) {}
    }
}
