using System.Globalization;
using System.Reflection;
using TypeTooling.ClassifiedText;

namespace TypeTooling.DotNet.RawTypes.Reflection
{
    public class ReflectionDotNetRawProperty : DotNetRawProperty
    {
        private readonly PropertyInfo propertyInfo;

        public ReflectionDotNetRawProperty(PropertyInfo propertyInfo)
        {
            this.propertyInfo = propertyInfo;
        }

        public override string Name => this.propertyInfo.Name;

        public override DotNetRawType PropertyType => new ReflectionDotNetRawType(this.propertyInfo.PropertyType);

        public override bool CanRead => this.propertyInfo.CanRead;

        public override bool CanWrite => this.propertyInfo.CanWrite;

        public override IEnumerable<DotNetRawCustomAttribute> GetCustomAttributes()
        {
            foreach (CustomAttributeData customAttributeData in this.propertyInfo.CustomAttributes)
            {
                yield return new ReflectionDotNetRawCustomAttribute(customAttributeData);
            }
        }

        public PropertyInfo PropertyInfo => this.propertyInfo;

        public override Task<ClassifiedTextMarkup?> GetDescriptionAsync(
            CultureInfo preferredCulture, CancellationToken cancellationToken)
        {
            return Task.FromResult((ClassifiedTextMarkup?)null);
        }
    }
}
