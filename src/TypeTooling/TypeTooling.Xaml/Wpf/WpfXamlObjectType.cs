using System;
using System.Collections.Generic;
using TypeTooling.DotNet.RawTypes;
using TypeTooling.Types;

namespace TypeTooling.Xaml.Wpf
{
    public class WpfXamlObjectType : XamlObjectType {

        public WpfXamlObjectType(XamlTypeToolingProvider typeToolingProvider, DotNetRawType rawType, DotNetRawType? companionType) :
            base(typeToolingProvider, rawType, companionType) {
        }

        protected override string? GetContentPropertyNameFromAttributes(IEnumerable<DotNetRawCustomAttribute> attributes) {
            return XamlTypeToolingProvider.GetAttributeValue(attributes, WpfXamlTypeToolingProvider.ContentPropertyAttribute, "Name") as string;
        }

        protected override CustomLiteralParser? GetCustomLiteralManagerFromAttributes(IEnumerable<DotNetRawCustomAttribute> attributes) {
            var typeConverterType = XamlTypeToolingProvider.GetAttributeValue(attributes, WpfXamlTypeToolingProvider.TypeConverterAttribute, "type") as Type;

            // TODO: Support TypeConverter type name being specified not only the type itself
            if (typeConverterType == null)
                return null;

            // TODO: Call CanConvertFrom to ensure converter can convert from strings

            return new WpfCustomLiteralParser(XamlTypeToolingProvider, typeConverterType);

            /*
            foreach (CustomAttributeData attribute in attributes) {
                if (attribute.AttributeType.FullName != "System.ComponentModel.TypeConverterAttribute")
                    continue;

                object typeConverterTypeObject = GetAttributeValue(attribute, "type");
                if (!(typeConverterTypeObject is Type typeConverterType))
                    continue;

                // TODO: Call CanConvertFrom to ensure converter can convert from strings

                MethodInfo convertFromMethod = typeConverterType.GetMethod("ConvertFromInvariantString");
                object typeConverter = Activator.CreateInstance(typeConverterType);

                return new FormsTypeParserCustomLiteralParser(typeConverter, convertFromMethod);
            }

            return null;
            */
        }
    }
}
