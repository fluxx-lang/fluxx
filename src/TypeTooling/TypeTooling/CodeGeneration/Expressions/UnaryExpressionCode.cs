using TypeTooling.CodeGeneration.Operators;

namespace TypeTooling.CodeGeneration.Expressions
{
    public class UnaryExpressionCode : ExpressionCode
    {
        private readonly UnaryOperator _operator;
        private readonly ExpressionCode _operand;

        public UnaryExpressionCode(UnaryOperator unaryOperator, ExpressionCode operand)
        {
            _operator = unaryOperator;
            _operand = operand;
        }

        public UnaryOperator Operator => _operator;

        public ExpressionCode Operand => _operand;
    }
}
