using System.Collections.Generic;
using TypeTooling;
using TypeTooling.DotNet.RawTypes;
using TypeTooling.Helper;
using TypeTooling.Types;


namespace TypeTooling.Xaml {
    public class XamlAttachedType : AttachedTypeLazyLoaded {
        private readonly XamlTypeToolingProvider _xamlTypeToolingProvider;
        private readonly DotNetRawType _rawType;


        // TODO: Support companionType, eventually
        public XamlAttachedType(XamlTypeToolingProvider typeToolingProvider, DotNetRawType rawType, DotNetRawType? companionType) {
            _xamlTypeToolingProvider = typeToolingProvider;
            _rawType = rawType;
        }

        public XamlTypeToolingProvider XamlTypeToolingProvider => _xamlTypeToolingProvider;

        protected override AttachedTypeData DoGetData() {
            TypeToolingEnvironment typeToolingEnvironment = _xamlTypeToolingProvider.TypeToolingEnvironment;

            // TODO: Maybe change implementation to look for "public static readonly property of type BindableProperty" instead
            var properties = new List<AttachedProperty>();
            foreach (DotNetRawMethod methodDescriptor in _rawType.GetPublicMethods()) {
                string methodName = methodDescriptor.Name;

                if (!methodName.StartsWith("Set"))
                    continue;

                if (!methodDescriptor.IsStatic)
                    continue;

                DotNetRawParameter[] parameters = methodDescriptor.GetParameters();
                if (parameters.Length != 2)
                    return null;

                // The first parameter to the setter method indicates the type of target objects that this property is allowed to be attached to
                DotNetRawType targetRawType = parameters[0].ParameterType;
                TypeToolingType? targetType = typeToolingEnvironment.GetType(targetRawType);

                // If the target type is null or not an ObjectType--both of which shouldn't typically happen--just ignore the property
                if (! (targetType is ObjectType targetObjectType))
                    continue;

                // The second parameter to the setter method indicates the property type
                DotNetRawType propertyRawType = parameters[1].ParameterType;
                TypeToolingType? propertyType = typeToolingEnvironment.GetType(propertyRawType);

                // Property type should rarely be null, but if it is for some reason, ignore the property
                if (propertyType == null)
                    continue;

                // The property name comes after the "Set" prefix
                string propertyName = methodName.Substring(3);

                properties.Add(new XamlAttachedProperty(this, propertyName, methodDescriptor, propertyType, targetObjectType));
            }

            // TODO: support base types, assuming XAML inherits attached properties
            IReadOnlyCollection<AttachedType> baseTypes = new List<AttachedType>();

            return new AttachedTypeData(fullName: _rawType.FullName, attachedProperties: properties, baseTypes: baseTypes);
        }
    }
}
