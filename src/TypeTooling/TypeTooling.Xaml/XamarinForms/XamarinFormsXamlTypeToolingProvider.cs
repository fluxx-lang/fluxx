using TypeTooling.DotNet.RawTypes;
using TypeTooling.RawTypes;
using TypeTooling.Types;

namespace TypeTooling.Xaml.XamarinForms {
    public class XamarinFormsXamlTypeToolingProvider : XamlTypeToolingProvider {
        private DotNetRawType? _bindableObjectType;

        public const string ContentPropertyAttribute = "Xamarin.Forms.ContentPropertyAttribute";
        public const string TypeConverterAttribute = "Xamarin.Forms.TypeConverterAttribute";

        public XamarinFormsXamlTypeToolingProvider(TypeToolingEnvironment typeToolingEnvironment) : base(typeToolingEnvironment) {
        }

        public override Platform Platform => Platform.Forms;

        private DotNetRawType GetBindableObjectType() {
            if (_bindableObjectType == null)
                _bindableObjectType = GetRequiredRawType("Xamarin.Forms.BindableObject");
            return _bindableObjectType;
        }

        public override TypeToolingType? ProvideType(RawType rawType, RawType? companionRawType) {
            if (!(rawType is DotNetRawType type))
                return null;

            // TODO: Fix this up to return FormsXamlObjectType, for right kind of custom literals

            if (IsMyType(type))
                return new XamarinFormsXamlObjectType(this, type, (DotNetRawType?)companionRawType);

            return null;
        }

        public override AttachedType? ProvideAttachedType(RawType rawType, RawType? companionRawType) {
            if (!(rawType is DotNetRawType dotNetTypeDescriptor))
                return null;

            var companionDotNetTypeDescriptor = (DotNetRawType?)companionRawType;

            // TODO: Change to check all methods instead for Get/Set method with first parameter of my type
            if (IsMyType(dotNetTypeDescriptor))
                return new XamarinFormsXamlAttachedType(this, dotNetTypeDescriptor, companionDotNetTypeDescriptor);

            return null;
        }

        private bool IsMyType(DotNetRawType rawType) {
            return rawType.IsAssignableTo(GetBindableObjectType());
        }
    }
}
