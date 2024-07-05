using TypeTooling.RawTypes;

namespace TypeTooling.CodeGeneration.Expressions
{
    public class GetPropertyCode : ExpressionCode
    {
        public ExpressionCode? ObjectExpression { get; }
        public RawProperty Property { get; }

        public GetPropertyCode(ExpressionCode? objectExpression, RawProperty property)
        {
            ObjectExpression = objectExpression;
            Property = property;
        }
    }
}
