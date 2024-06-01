using System.Collections.Generic;
using TypeTooling.CodeGeneration.Expressions;
using TypeTooling.RawTypes;

namespace TypeTooling.Types {
    public class EnhancedObjectType : ObjectType {
        private readonly ObjectType _originalType;
        private readonly TypeOverride _typeOverride;


        public EnhancedObjectType(ObjectType originalType) {
            _originalType = originalType;
            _typeOverride = new TypeOverride(originalType);
        }

        public TypeOverride TypeOverride => _typeOverride;

        public override string FullName => _originalType.FullName;

        public override RawType UnderlyingType => _originalType.UnderlyingType;

        public override CustomLiteralParser? GetCustomLiteralParser() {
            return _typeOverride.GetCustomLiteralParser();
        }

        public override Visualizer? GetVisualizer() {
            return _typeOverride.GetVisualizer();
        }

        public override IReadOnlyCollection<ObjectProperty> Properties => _originalType.Properties;

        /// <summary>
        /// Return the content property for this object or null if the object can't have content other than its named properties.
        /// </summary>
        public override ObjectProperty? ContentProperty => _originalType.ContentProperty;

        public override ExpressionCode GetCreateObjectCode(PropertyValue<string, ExpressionCode>[] propertyValues, PropertyValue<AttachedProperty, ExpressionCode>[] attachedPropertyValues) =>
            _originalType.GetCreateObjectCode(propertyValues, attachedPropertyValues);

        public override GetPropertyCode GetGetPropertyCode(ExpressionCode instance, string property) =>
            _originalType.GetGetPropertyCode(instance, property);

        public override InterpretedObjectCreator? GetInterpretedObjectCreator(ObjectProperty[] properties,
            AttachedProperty[] attachedProperties) {
            return _originalType.GetInterpretedObjectCreator(properties, attachedProperties);
        }

        public override ObjectPropertyReader GetPropertyReader(ObjectProperty property) {
            return _originalType.GetPropertyReader(property);
        }

        public override IReadOnlyCollection<ObjectType> GetBaseTypes() {
            return _originalType.GetBaseTypes();
        }
    }
}
