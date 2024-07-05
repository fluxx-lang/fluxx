using TypeTooling.CodeGeneration.Operators;

namespace TypeTooling.CodeGeneration.Expressions
{
    public class UnaryExpressionCode : ExpressionCode
    {
        private readonly UnaryOperator @operator;
        private readonly ExpressionCode operand;

        public UnaryExpressionCode(UnaryOperator unaryOperator, ExpressionCode operand)
        {
            this.@operator = unaryOperator;
            this.operand = operand;
        }

        public UnaryOperator Operator => this.@operator;

        public ExpressionCode Operand => this.operand;
    }
}
