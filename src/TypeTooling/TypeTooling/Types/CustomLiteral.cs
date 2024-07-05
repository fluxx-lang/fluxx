using System.Collections.Immutable;
using TypeTooling.CodeGeneration;
using TypeTooling.CodeGeneration.Expressions;

namespace TypeTooling.Types
{
    public class CustomLiteral
    {
        private readonly ImmutableArray<Diagnostic>? diagnostics;
        private readonly ExpressionAndHelpersCode? expressionAndHelpersCode;

        public CustomLiteral(ImmutableArray<Diagnostic>? diagnostics, ExpressionAndHelpersCode? expressionAndHelpersCode)
        {
            this.diagnostics = diagnostics;
            this.expressionAndHelpersCode = expressionAndHelpersCode;
        }

        public CustomLiteral(ImmutableArray<Diagnostic>? diagnostics, ExpressionCode expressionCode)
        {
            this.diagnostics = diagnostics;
            this.expressionAndHelpersCode = Code.ExpressionAndHelpers(expressionCode);
        }

        public CustomLiteral(ExpressionAndHelpersCode expressionAndHelpersCode) : this(null, expressionAndHelpersCode) { }

        public CustomLiteral(ExpressionCode expressionCode) : this(null, expressionCode) { }

        public static CustomLiteral SingleError(string message)
        {
            return new CustomLiteral(Diagnostic.SingleError(message), (ExpressionAndHelpersCode?) null);
        }

        public ImmutableArray<Diagnostic>? Diagnostics => this.diagnostics;

        public ExpressionAndHelpersCode? ExpressionAndHelpersCode => this.expressionAndHelpersCode;
    }
}
