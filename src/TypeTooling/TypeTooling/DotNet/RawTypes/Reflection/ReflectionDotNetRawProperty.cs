using System.Globalization;
using System.Reflection;
using TypeTooling.ClassifiedText;

namespace TypeTooling.DotNet.RawTypes.Reflection
{
    public class ReflectionDotNetRawProperty : DotNetRawProperty
    {
        private readonly PropertyInfo _propertyInfo;

        public ReflectionDotNetRawProperty(PropertyInfo propertyInfo)
        {
            this._propertyInfo = propertyInfo;
        }

        public override string Name => this._propertyInfo.Name;

        public override DotNetRawType PropertyType => new ReflectionDotNetRawType(this._propertyInfo.PropertyType);

        public override bool CanRead => this._propertyInfo.CanRead;

        public override bool CanWrite => this._propertyInfo.CanWrite;

        public override IEnumerable<DotNetRawCustomAttribute> GetCustomAttributes()
        {
            foreach (CustomAttributeData customAttributeData in this._propertyInfo.CustomAttributes)
                yield return new ReflectionDotNetRawCustomAttribute(customAttributeData);
        }

        public PropertyInfo PropertyInfo => this._propertyInfo;

        public override Task<ClassifiedTextMarkup?> GetDescriptionAsync(CultureInfo preferredCulture,
            CancellationToken cancellationToken)
            {
            return Task.FromResult((ClassifiedTextMarkup?) null);
        }
    }
}
