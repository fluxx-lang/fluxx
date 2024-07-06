using TypeTooling.RawTypes;

namespace TypeTooling.CodeGeneration.Expressions
{
    public class NewObjectCode(RawType objectType, RawConstructor constructor, ExpressionCode[] constructorArguments, PropertyValue<RawProperty, ExpressionCode>[] propertyValues) : ExpressionCode
    {
        public RawType ObjectType { get; } = objectType;

        public RawConstructor Constructor { get; } = constructor;

        public ExpressionCode[] ConstructorArguments { get; } = constructorArguments;

        public PropertyValue<RawProperty, ExpressionCode>[] PropertyValues { get; } = propertyValues;
    }
}
