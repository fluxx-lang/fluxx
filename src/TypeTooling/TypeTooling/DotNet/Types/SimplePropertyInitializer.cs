using TypeTooling.DotNet.RawTypes;
using TypeTooling.DotNet.RawTypes.Reflection;

namespace TypeTooling.DotNet.Types
{
    public sealed class SimplePropertyInitializer : PropertyInitializer
    {
        private readonly DotNetRawProperty rawProperty;

        public SimplePropertyInitializer(DotNetRawProperty rawProperty)
        {
            this.rawProperty = rawProperty;
        }

        public override void Initialize(object obj, object propertyValue)
        {
            ((ReflectionDotNetRawProperty)this.rawProperty).PropertyInfo.SetValue(obj, propertyValue);
        }
    }
}
