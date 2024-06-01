using System;
using System.Collections.Generic;
using System.Linq;
using TypeTooling.DotNet.RawTypes;
using TypeTooling.RawTypes;
using TypeTooling.Types;
using TypeTooling.Xaml.CustomAttributes;

namespace TypeTooling.Xaml.Uwp {
    public class UwpXamlTypeToolingProvider : XamlTypeToolingProvider {
        private DotNetRawType? _dependencyObjectType;
        private readonly Lazy<DesignAttributeTable> _typeConverterCustomAttributes;
        private readonly Lazy<DesignAttributeTable> _otherCustomAttributes;

        public const string ContentPropertyAttribute = "Windows.UI.Xaml.Markup.ContentPropertyAttribute";

        public UwpXamlTypeToolingProvider(TypeToolingEnvironment typeToolingEnvironment) : base(typeToolingEnvironment) {
            _typeConverterCustomAttributes =
                new Lazy<DesignAttributeTable>(() => new UwpHostAttributeTableBuilder(this).CreateTable());
            _otherCustomAttributes =
                new Lazy<DesignAttributeTable>(() => new UwpEditorAttributeTableBuilder().CreateTable());
        }

        private DotNetRawType GetDependencyObjectType() {
            if (_dependencyObjectType == null)
                _dependencyObjectType = GetRequiredRawType("Windows.UI.Xaml.DependencyObject");
            return _dependencyObjectType;
        }

        public override TypeToolingType? ProvideType(RawType rawType, RawType? companionRawType) {
            if (!(rawType is DotNetRawType type))
                return null;

            var companionType = (DotNetRawType?) companionRawType;

            if (IsMyType(type))
                return new UwpXamlObjectType(this, type, companionType);

            return null;
        }

        public override AttachedType? ProvideAttachedType(RawType rawType, RawType? companionRawType) {
            if (!(rawType is DotNetRawType dotNetTypeDescriptor))
                return null;

            var companionDotNetTypeDescriptor = (DotNetRawType?) companionRawType;

            if (IsMyType(dotNetTypeDescriptor))
                return new UwpXamlAttachedType(this, dotNetTypeDescriptor, companionDotNetTypeDescriptor);

            return null;
        }

        private bool IsMyType(DotNetRawType rawType) {
            return rawType.IsAssignableTo(GetDependencyObjectType());
        }

#if false
        protected static List<MethodInfo> GetAttachedPropertySetters(Type dotNetType) {
            // TODO: Maybe change implementation to look for "public static readonly property of type BindableProperty" instead
            var properties = new List<AttachedProperty>();
            foreach (FieldInfo fieldInfo in dotNetType.GetFields()) {
                if (! fieldInfo.IsStatic)
                    continue;

                if (fieldInfo.GetType().FullName != "Windows.UI.Xaml.DependencyProperty")
                    continue;

                if ()


                string methodName = methodInfo.Name;

                if (!methodName.StartsWith("Set"))
                    continue;

                if (!methodInfo.IsStatic)
                    continue;

                ParameterInfo[] parameters = methodInfo.GetParameters();
                if (parameters.Length != 2)
                    return null;

                // The first parameter to the setter method indicates the type of target objects that this property is allowed to be attached to
                Type targetDotNetType = parameters[0].ParameterType;
                TypeToolingType targetType = typeToolingEnvironment.GetType(targetDotNetType);

                // If the target type is null or not an ObjectType--both of which shouldn't typically happen--just ignore the property
                if (!(targetType is ObjectType targetObjectType))
                    continue;

                // The second parameter to the setter method indicates the property type
                Type propertyDotNetType = parameters[1].ParameterType;
                TypeToolingType propertyType = typeToolingEnvironment.GetType(propertyDotNetType);

                // Property type should rarely be null, but if it is for some reason, ignore the property
                if (propertyType == null)
                    continue;

                // The property name comes after the "Set" prefix
                string propertyName = methodName.Substring(3);

                properties.Add(new XamlAttachedProperty(this, propertyName, methodInfo, propertyType, targetObjectType));
            }

            // TODO: support base types, assuming XAML inherits attached properties
            IReadOnlyCollection<AttachedType> baseTypes = new List<AttachedType>();

            return new AttachedTypeData(fullName: _dotNetType.FullName, attachedProperties: properties, baseTypes: baseTypes);
        }
#endif

        public override Platform Platform => Platform.Uwp;

        public override IEnumerable<object> GetSyntheticCustomAttributes(string typeName) {
            IEnumerable<object> attributes1 = _typeConverterCustomAttributes.Value.GetCustomAttributes(typeName);
            IEnumerable<object> attributes2 = _otherCustomAttributes.Value.GetCustomAttributes(typeName);

            return attributes1.Concat(attributes2);
        }

        public override IEnumerable<object> GetSyntheticCustomAttributes(string typeName, string memberName) {
            IEnumerable<object> attributes1 = _typeConverterCustomAttributes.Value.GetCustomAttributes(typeName, memberName);
            IEnumerable<object> attributes2 = _otherCustomAttributes.Value.GetCustomAttributes(typeName, memberName);

            return attributes1.Concat(attributes2);
        }

        /*
        public override bool IsContentPropertyAttribute(CustomAttributeData attribute) {
            return attribute.AttributeType.FullName == "Windows.UI.Xaml.Markup.ContentPropertyAttribute";
        }

        // TODO: REMOVE THIS AS NO ATTRIBUTE FROM UWP
        public override bool IsTypeConverterAttribute(CustomAttributeData attribute) {
            string attributeName = attribute.AttributeType.FullName;
            return attributeName == "System.ComponentModel.TypeConverterAttribute";
        }
        */
    }
}
