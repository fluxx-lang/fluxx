using TypeTooling.CodeGeneration.Operators;

namespace TypeTooling.CodeGeneration.Expressions
{
    public class UnaryExpressionCode(UnaryOperator unaryOperator, ExpressionCode operand) : ExpressionCode
    {
        private readonly UnaryOperator @operator = unaryOperator;
        private readonly ExpressionCode operand = operand;

        public UnaryOperator Operator => this.@operator;

        public ExpressionCode Operand => this.operand;
    }
}
