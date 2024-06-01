using TypeTooling.CodeGeneration.Expressions;

namespace TypeTooling.CodeGeneration {
    public class ExpressionAndHelpersCode {
        private readonly ExpressionCode _expressionCode;

        public ExpressionAndHelpersCode(ExpressionCode expressionCode) {
            _expressionCode = expressionCode;
        }

        public ExpressionCode Expression => _expressionCode;
    }
}
