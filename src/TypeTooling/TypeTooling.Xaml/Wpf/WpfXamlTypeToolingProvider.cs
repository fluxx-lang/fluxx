using System;
using TypeTooling.DotNet.RawTypes;
using TypeTooling.RawTypes;
using TypeTooling.Types;

namespace TypeTooling.Xaml.Wpf {
    public class WpfXamlTypeToolingProvider : XamlTypeToolingProvider {
        private DotNetRawType? _dependencyObjectType;

        public const string ContentPropertyAttribute = "System.Windows.Markup.ContentPropertyAttribute";
        public const string TypeConverterAttribute = "System.ComponentModel.TypeConverterAttribute";

        public WpfXamlTypeToolingProvider(TypeToolingEnvironment typeToolingEnvironment) : base(typeToolingEnvironment) {
        }

        private DotNetRawType GetDependencyObjectType() {
            if (_dependencyObjectType == null)
                _dependencyObjectType = GetRequiredRawType("System.Windows.DependencyObject");
            return _dependencyObjectType;
        }

        public override TypeToolingType? ProvideType(RawType rawType, RawType? companionType) {
            if (!(rawType is DotNetRawType dotNetRawType))
                return null;

            // TODO: FIX THIS UP
#if false
            var companionDotNetRawType = (DotNetRawType) companionDotNetRawType;
            string typeName = dotNetRawType.FullName;

            if (typeName == "System.Windows.Controls.UIElementCollection") {
                MethodInfo methodInfo = dotNetRawType.GetMethod("Add");
                if (methodInfo == null)
                    throw new Exception("Add method on System.Windows.Controls.UIElementCollection unexpectedly not found");

                ParameterInfo[] parameters = methodInfo.GetParameters();
                if (parameters.Length == 0)
                    throw new Exception("UIElementCollection.Add method unexpectedly takes 0 paraemters");

                // Do this to get the type UIElemenet, as we can load the type directly
                Type uiElementType = parameters[0].ParameterType;

                return new DotNetCollectionType(TypeToolingEnvironment, dotNetRawType, uiElementType);
            }
#endif

            if (IsMyType(dotNetRawType))
                return new WpfXamlObjectType(this, dotNetRawType, (DotNetRawType?)companionType);

            return null;
        }

        public override Platform Platform => Platform.Wpf;

        private bool IsMyType(Type type) {
            return false;
        }

        private bool IsMyType(DotNetRawType rawType) {
            return rawType.IsAssignableTo(GetDependencyObjectType());
        }

        /*
        public override bool IsContentPropertyAttribute(CustomAttributeData attribute) {
            return attribute.AttributeType.FullName == "System.Windows.Markup.ContentPropertyAttribute";
        }

        public override bool IsTypeConverterAttribute(CustomAttributeData attribute) {
            return attribute.AttributeType.FullName == "System.ComponentModel.TypeConverterAttribute";
        }
        */
    }
}
