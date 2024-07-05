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
            _operator = @operator;
            _leftOperand = leftOperand;
            _rightOperand = rightOperand;
        }

        public BinaryOperator Operator => _operator;

        public ExpressionCode LeftOperand => _leftOperand;

        public ExpressionCode RightOperand => _rightOperand;
    }
}
