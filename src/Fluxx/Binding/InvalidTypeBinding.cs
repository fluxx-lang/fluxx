using Fluxx.Api;

namespace Fluxx.Binding
{
    public class InvalidTypeBinding(QualifiableName typeName) : TypeBinding(typeName, TypeFlags.None)
    {
        public static InvalidTypeBinding Instance = new InvalidTypeBinding(new QualifiableName("InvalidType"));
    }
}
