using System.Collections.Generic;
using TypeTooling.DotNet.RawTypes;

namespace TypeTooling.Xaml.Uwp
{
    public class UwpXamlObjectType : XamlObjectType {
        public UwpXamlObjectType(XamlTypeToolingProvider typeToolingProvider, DotNetRawType rawType, DotNetRawType? companionType) :
            base(typeToolingProvider, rawType, companionType) {
        }

        protected override string? GetContentPropertyNameFromAttributes(IEnumerable<DotNetRawCustomAttribute> attributes) {
            return XamlTypeToolingProvider.GetAttributeValue(attributes, UwpXamlTypeToolingProvider.ContentPropertyAttribute, "Name") as string;
        }
    }
}
