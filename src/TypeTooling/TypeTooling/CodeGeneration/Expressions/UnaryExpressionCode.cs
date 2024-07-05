using TypeTooling.CodeGeneration.Operators;

namespace TypeTooling.CodeGeneration.Expressions
{
    public class UnaryExpressionCode : ExpressionCode
    {
        private readonly UnaryOperator _operator;
        private readonly ExpressionCode _operand;

        public UnaryExpressionCode(UnaryOperator unaryOperator, ExpressionCode operand)
        {
            this._operator = unaryOperator;
            this._operand = operand;
        }

        public UnaryOperator Operator => this._operator;

        public ExpressionCode Operand => this._operand;
    }
}
