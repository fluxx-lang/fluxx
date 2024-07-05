using System.Collections.Generic;
using TypeTooling.CodeGeneration.Expressions;
using TypeTooling.Images;

namespace TypeTooling.Types {
    public abstract class ObjectType : TypeToolingType
    {
        /// <remarks>
        /// These properties should be minimally equatable to the same property for another object of the same type. This includes
        /// for base class properties for two different derived types.
        /// </remarks>
        public abstract IReadOnlyCollection<ObjectProperty> Properties { get; }

        /// <summary>
        /// Return the content property for this object or null if the object can't have content other than its named properties.
        /// </summary>
        public abstract ObjectProperty? ContentProperty { get; }

        public abstract ExpressionCode GetCreateObjectCode(
            PropertyValue<string, ExpressionCode>[] propertyValues, PropertyValue<AttachedProperty, ExpressionCode>[] attachedPropertyValues);

        public abstract GetPropertyCode GetGetPropertyCode(ExpressionCode instance, string property);

        public abstract InterpretedObjectCreator? GetInterpretedObjectCreator(
            ObjectProperty[] properties, AttachedProperty[] attachedProperties);
    
        public abstract ObjectPropertyReader GetPropertyReader(ObjectProperty property);

        public abstract IReadOnlyCollection<ObjectType> GetBaseTypes();

        public virtual Image? GetIcon() => null;
    }
}
