using TypeTooling.CodeGeneration.Operators;

namespace TypeTooling.CodeGeneration.Expressions
{
    public class BinaryExpressionCode : ExpressionCode
    {
        private readonly BinaryOperator @operator;
        private readonly ExpressionCode leftOperand;
        private readonly ExpressionCode rightOperand;

        public BinaryExpressionCode(BinaryOperator @operator, ExpressionCode leftOperand, ExpressionCode rightOperand)
        {
            this.@operator = @operator;
            this.leftOperand = leftOperand;
            this.rightOperand = rightOperand;
        }

        public BinaryOperator Operator => this.@operator;

        public ExpressionCode LeftOperand => this.leftOperand;

        public ExpressionCode RightOperand => this.rightOperand;
    }
}
