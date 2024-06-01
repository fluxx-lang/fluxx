using System;
using System.Collections.Generic;
using TypeTooling.DotNet.RawTypes;
using TypeTooling.Types;

namespace TypeTooling.Xaml.XamarinForms {
    public class XamarinFormsXamlObjectType : XamlObjectType {
        public XamarinFormsXamlObjectType(XamlTypeToolingProvider typeToolingProvider, DotNetRawType rawType, DotNetRawType? companionRawType) :
            base(typeToolingProvider, rawType, companionRawType) {
        }

        protected override string? GetContentPropertyNameFromAttributes(IEnumerable<DotNetRawCustomAttribute> attributes) {
            return XamlTypeToolingProvider.GetAttributeValue(attributes, XamarinFormsXamlTypeToolingProvider.ContentPropertyAttribute, "Name") as string;
        }

        protected override CustomLiteralParser? GetCustomLiteralManagerFromAttributes(IEnumerable<DotNetRawCustomAttribute> attributes) {
            var typeConverterType = XamlTypeToolingProvider.GetAttributeValue(attributes, XamarinFormsXamlTypeToolingProvider.TypeConverterAttribute, "type") as Type;

            // TODO: Support TypeConverter type name being specified not only the type itself
            if (typeConverterType == null)
                return null;

            // TODO: Call CanConvertFrom to ensure converter can convert from strings

            return new XamarinFormsCustomLiteralParser(XamlTypeToolingProvider, typeConverterType);
        }
    }
}
