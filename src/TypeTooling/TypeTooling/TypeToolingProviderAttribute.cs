namespace TypeTooling
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class TypeToolingProviderAttribute(Type providerType) : Attribute
    {
        public Type ProviderType { get; } = providerType;
    }
}
