using System;

namespace TypeTooling
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class TypeToolingEnhancerAttribute : Attribute {
        public System.Type EnhancerType { get; }

        public TypeToolingEnhancerAttribute(System.Type enhancerType) {
            EnhancerType = enhancerType;
        }
    }
}
