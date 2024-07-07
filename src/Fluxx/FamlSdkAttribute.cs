using System;

namespace Faml
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class FamlSdkAttribute : Attribute {
        public Type SdkType { get; }

        public FamlSdkAttribute(Type sdkType) {
            this.SdkType = sdkType;
        }
    }
}
