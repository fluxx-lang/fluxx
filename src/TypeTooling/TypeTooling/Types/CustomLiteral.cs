using System.Collections.Immutable;
using TypeTooling.CodeGeneration;
using TypeTooling.CodeGeneration.Expressions;

namespace TypeTooling.Types
{
    public class CustomLiteral
    {
        public CustomLiteral(ImmutableArray<Diagnostic>? diagnostics, ExpressionAndHelpersCode? expressionAndHelpersCode)
        {
            this.Diagnostics = diagnostics;
            this.ExpressionAndHelpersCode = expressionAndHelpersCode;
        }

        public CustomLiteral(ImmutableArray<Diagnostic>? diagnostics, ExpressionCode expressionCode)
        {
            this.Diagnostics = diagnostics;
            this.ExpressionAndHelpersCode = Code.ExpressionAndHelpers(expressionCode);
        }

        public CustomLiteral(ExpressionAndHelpersCode expressionAndHelpersCode)
            : this(null, expressionAndHelpersCode) { }

        public CustomLiteral(ExpressionCode expressionCode)
            : this(null, expressionCode) { }

        public ImmutableArray<Diagnostic>? Diagnostics { get; }

        public ExpressionAndHelpersCode? ExpressionAndHelpersCode { get; }

        public static CustomLiteral SingleError(string message)
        {
            return new CustomLiteral(Diagnostic.SingleError(message), (ExpressionAndHelpersCode?) null);
        }
    }
}
