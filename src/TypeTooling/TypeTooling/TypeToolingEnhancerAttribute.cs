namespace TypeTooling
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class TypeToolingEnhancerAttribute(System.Type enhancerType) : Attribute
    {
        public System.Type EnhancerType { get; } = enhancerType;
    }
}
