using TypeTooling.RawTypes;

namespace TypeTooling.CodeGeneration.Expressions
{
    public class GetPropertyCode : ExpressionCode
    {
        public ExpressionCode? ObjectExpression { get; }
        public RawProperty Property { get; }

        public GetPropertyCode(ExpressionCode? objectExpression, RawProperty property)
        {
            this.ObjectExpression = objectExpression;
            this.Property = property;
        }
    }
}
