using TypeTooling.DotNet.RawTypes;

namespace TypeTooling.Xaml.XamarinForms
{
    public class XamarinFormsXamlAttachedType : XamlAttachedType {
        public XamarinFormsXamlAttachedType(XamlTypeToolingProvider typeToolingProvider, DotNetRawType rawType, DotNetRawType? companionRawType) :
            base(typeToolingProvider, rawType, companionRawType) {
        }
    }
}
