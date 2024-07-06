using TypeTooling.RawTypes;

namespace TypeTooling.CodeGeneration.Expressions
{
    public class GetPropertyCode(ExpressionCode? objectExpression, RawProperty property) : ExpressionCode
    {
        public ExpressionCode? ObjectExpression { get; } = objectExpression;

        public RawProperty Property { get; } = property;
    }
}
