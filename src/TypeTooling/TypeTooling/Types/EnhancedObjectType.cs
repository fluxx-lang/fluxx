using System.Collections.Generic;
using TypeTooling.CodeGeneration.Expressions;
using TypeTooling.RawTypes;

namespace TypeTooling.Types
{
    public class EnhancedObjectType : ObjectType
    {
        private readonly ObjectType _originalType;
        private readonly TypeOverride _typeOverride;


        public EnhancedObjectType(ObjectType originalType)
        {
            this._originalType = originalType;
            this._typeOverride = new TypeOverride(originalType);
        }

        public TypeOverride TypeOverride => this._typeOverride;

        public override string FullName => this._originalType.FullName;

        public override RawType UnderlyingType => this._originalType.UnderlyingType;

        public override CustomLiteralParser? GetCustomLiteralParser()
        {
            return this._typeOverride.GetCustomLiteralParser();
        }

        public override Visualizer? GetVisualizer()
        {
            return this._typeOverride.GetVisualizer();
        }

        public override IReadOnlyCollection<ObjectProperty> Properties => this._originalType.Properties;

        /// <summary>
        /// Return the content property for this object or null if the object can't have content other than its named properties.
        /// </summary>
        public override ObjectProperty? ContentProperty => this._originalType.ContentProperty;

        public override ExpressionCode GetCreateObjectCode(PropertyValue<string, ExpressionCode>[] propertyValues, PropertyValue<AttachedProperty, ExpressionCode>[] attachedPropertyValues) =>
            this._originalType.GetCreateObjectCode(propertyValues, attachedPropertyValues);

        public override GetPropertyCode GetGetPropertyCode(ExpressionCode instance, string property) =>
            this._originalType.GetGetPropertyCode(instance, property);

        public override InterpretedObjectCreator? GetInterpretedObjectCreator(ObjectProperty[] properties,
            AttachedProperty[] attachedProperties)
            {
            return this._originalType.GetInterpretedObjectCreator(properties, attachedProperties);
        }

        public override ObjectPropertyReader GetPropertyReader(ObjectProperty property)
        {
            return this._originalType.GetPropertyReader(property);
        }

        public override IReadOnlyCollection<ObjectType> GetBaseTypes()
        {
            return this._originalType.GetBaseTypes();
        }
    }
}
