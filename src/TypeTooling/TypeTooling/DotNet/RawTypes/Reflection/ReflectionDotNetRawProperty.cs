using System.Globalization;
using System.Reflection;
using TypeTooling.ClassifiedText;

namespace TypeTooling.DotNet.RawTypes.Reflection
{
    public class ReflectionDotNetRawProperty : DotNetRawProperty {
        private readonly PropertyInfo _propertyInfo;

        public ReflectionDotNetRawProperty(PropertyInfo propertyInfo) {
            _propertyInfo = propertyInfo;
        }

        public override string Name => _propertyInfo.Name;

        public override DotNetRawType PropertyType => new ReflectionDotNetRawType(_propertyInfo.PropertyType);

        public override bool CanRead => _propertyInfo.CanRead;

        public override bool CanWrite => _propertyInfo.CanWrite;

        public override IEnumerable<DotNetRawCustomAttribute> GetCustomAttributes() {
            foreach (CustomAttributeData customAttributeData in _propertyInfo.CustomAttributes)
                yield return new ReflectionDotNetRawCustomAttribute(customAttributeData);
        }

        public PropertyInfo PropertyInfo => _propertyInfo;

        public override Task<ClassifiedTextMarkup?> GetDescriptionAsync(CultureInfo preferredCulture,
            CancellationToken cancellationToken) {
            return Task.FromResult((ClassifiedTextMarkup?) null);
        }
    }
}
