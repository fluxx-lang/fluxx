using System.Collections.Generic;
using TypeTooling.CodeGeneration.Expressions;
using TypeTooling.RawTypes;

namespace TypeTooling.Types
{
    public class EnhancedObjectType : ObjectType
    {
        private readonly ObjectType originalType;
        private readonly TypeOverride typeOverride;


        public EnhancedObjectType(ObjectType originalType)
        {
            this.originalType = originalType;
            this.typeOverride = new TypeOverride(originalType);
        }

        public TypeOverride TypeOverride => this.typeOverride;

        public override string FullName => this.originalType.FullName;

        public override RawType UnderlyingType => this.originalType.UnderlyingType;

        public override CustomLiteralParser? GetCustomLiteralParser()
        {
            return this.typeOverride.GetCustomLiteralParser();
        }

        public override Visualizer? GetVisualizer()
        {
            return this.typeOverride.GetVisualizer();
        }

        public override IReadOnlyCollection<ObjectProperty> Properties => this.originalType.Properties;

        /// <summary>
        /// Return the content property for this object or null if the object can't have content other than its named properties.
        /// </summary>
        public override ObjectProperty? ContentProperty => this.originalType.ContentProperty;

        public override ExpressionCode GetCreateObjectCode(PropertyValue<string, ExpressionCode>[] propertyValues, PropertyValue<AttachedProperty, ExpressionCode>[] attachedPropertyValues) =>
            this.originalType.GetCreateObjectCode(propertyValues, attachedPropertyValues);

        public override GetPropertyCode GetGetPropertyCode(ExpressionCode instance, string property) =>
            this.originalType.GetGetPropertyCode(instance, property);

        public override InterpretedObjectCreator? GetInterpretedObjectCreator(ObjectProperty[] properties,
            AttachedProperty[] attachedProperties)
            {
            return this.originalType.GetInterpretedObjectCreator(properties, attachedProperties);
        }

        public override ObjectPropertyReader GetPropertyReader(ObjectProperty property)
        {
            return this.originalType.GetPropertyReader(property);
        }

        public override IReadOnlyCollection<ObjectType> GetBaseTypes()
        {
            return this.originalType.GetBaseTypes();
        }
    }
}
