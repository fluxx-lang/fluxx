using TypeTooling.DotNet.RawTypes;

namespace TypeTooling.Xaml.Uwp
{
    public class UwpXamlAttachedType : XamlAttachedType {
        public UwpXamlAttachedType(XamlTypeToolingProvider typeToolingProvider, DotNetRawType rawType, DotNetRawType? companionRawType) :
            base(typeToolingProvider, rawType, companionRawType) {
        }
    }
}
