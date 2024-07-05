using TypeTooling.RawTypes;

namespace TypeTooling.CodeGeneration.Expressions
{
    public class NewObjectCode : ExpressionCode
    {
        public RawType ObjectType { get; }
        public RawConstructor Constructor { get; }
        public ExpressionCode[] ConstructorArguments { get; }
        public PropertyValue<RawProperty, ExpressionCode>[] PropertyValues { get; }

        public NewObjectCode(RawType objectType, RawConstructor constructor, ExpressionCode[] constructorArguments, PropertyValue<RawProperty, ExpressionCode>[] propertyValues)
        {
            ObjectType = objectType;
            Constructor = constructor;
            ConstructorArguments = constructorArguments;
            PropertyValues = propertyValues;
        }
    }
}
