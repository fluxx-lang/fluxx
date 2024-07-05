using System;

namespace TypeTooling
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class TypeToolingProviderAttribute : Attribute
    {
        public Type ProviderType { get; }

        public TypeToolingProviderAttribute(Type providerType)
        {
            this.ProviderType = providerType;
        }
    }
}
