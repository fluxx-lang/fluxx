using TypeTooling.CodeGeneration.Operators;

namespace TypeTooling.CodeGeneration.Expressions
{
    public class BinaryExpressionCode : ExpressionCode
    {
        private readonly BinaryOperator _operator;
        private readonly ExpressionCode _leftOperand;
        private readonly ExpressionCode _rightOperand;

        public BinaryExpressionCode(BinaryOperator @operator, ExpressionCode leftOperand, ExpressionCode rightOperand)
        {
            this._operator = @operator;
            this._leftOperand = leftOperand;
            this._rightOperand = rightOperand;
        }

        public BinaryOperator Operator => this._operator;

        public ExpressionCode LeftOperand => this._leftOperand;

        public ExpressionCode RightOperand => this._rightOperand;
    }
}
