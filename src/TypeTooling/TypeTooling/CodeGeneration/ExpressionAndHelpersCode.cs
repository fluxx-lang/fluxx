using TypeTooling.CodeGeneration.Expressions;

namespace TypeTooling.CodeGeneration
{
    public class ExpressionAndHelpersCode
    {
        private readonly ExpressionCode expressionCode;

        public ExpressionAndHelpersCode(ExpressionCode expressionCode)
        {
            this.expressionCode = expressionCode;
        }

        public ExpressionCode Expression => this.expressionCode;
    }
}
