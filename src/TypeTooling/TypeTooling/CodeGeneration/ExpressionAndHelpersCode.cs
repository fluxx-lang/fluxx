using TypeTooling.CodeGeneration.Expressions;

namespace TypeTooling.CodeGeneration
{
    public class ExpressionAndHelpersCode
    {
        private readonly ExpressionCode _expressionCode;

        public ExpressionAndHelpersCode(ExpressionCode expressionCode)
        {
            this._expressionCode = expressionCode;
        }

        public ExpressionCode Expression => this._expressionCode;
    }
}
