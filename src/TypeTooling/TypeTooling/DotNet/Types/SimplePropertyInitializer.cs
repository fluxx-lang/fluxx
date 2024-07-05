using TypeTooling.DotNet.RawTypes;
using TypeTooling.DotNet.RawTypes.Reflection;

namespace TypeTooling.DotNet.Types
{
    public sealed class SimplePropertyInitializer : PropertyInitializer
    {
        private readonly DotNetRawProperty _rawProperty;

        public SimplePropertyInitializer(DotNetRawProperty rawProperty)
        {
            _rawProperty = rawProperty;
        }

        public override void Initialize(object obj, object propertyValue)
        {
            ((ReflectionDotNetRawProperty) _rawProperty).PropertyInfo.SetValue(obj, propertyValue);
        }
    }
}
