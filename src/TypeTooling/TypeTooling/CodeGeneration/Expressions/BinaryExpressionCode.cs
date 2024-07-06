using TypeTooling.CodeGeneration.Operators;

namespace TypeTooling.CodeGeneration.Expressions
{
    public class BinaryExpressionCode(BinaryOperator @operator, ExpressionCode leftOperand, ExpressionCode rightOperand) : ExpressionCode
    {
        private readonly BinaryOperator @operator = @operator;
        private readonly ExpressionCode leftOperand = leftOperand;
        private readonly ExpressionCode rightOperand = rightOperand;

        public BinaryOperator Operator => this.@operator;

        public ExpressionCode LeftOperand => this.leftOperand;

        public ExpressionCode RightOperand => this.rightOperand;
    }
}
