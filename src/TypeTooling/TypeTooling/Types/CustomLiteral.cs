using System.Collections.Immutable;
using TypeTooling.CodeGeneration;
using TypeTooling.CodeGeneration.Expressions;

namespace TypeTooling.Types
{
    public class CustomLiteral
    {
        private readonly ImmutableArray<Diagnostic>? _diagnostics;
        private readonly ExpressionAndHelpersCode? _expressionAndHelpersCode;

        public CustomLiteral(ImmutableArray<Diagnostic>? diagnostics, ExpressionAndHelpersCode? expressionAndHelpersCode)
        {
            _diagnostics = diagnostics;
            _expressionAndHelpersCode = expressionAndHelpersCode;
        }

        public CustomLiteral(ImmutableArray<Diagnostic>? diagnostics, ExpressionCode expressionCode)
        {
            _diagnostics = diagnostics;
            _expressionAndHelpersCode = Code.ExpressionAndHelpers(expressionCode);
        }

        public CustomLiteral(ExpressionAndHelpersCode expressionAndHelpersCode) : this(null, expressionAndHelpersCode) { }

        public CustomLiteral(ExpressionCode expressionCode) : this(null, expressionCode) { }

        public static CustomLiteral SingleError(string message)
        {
            return new CustomLiteral(Diagnostic.SingleError(message), (ExpressionAndHelpersCode?) null);
        }

        public ImmutableArray<Diagnostic>? Diagnostics => _diagnostics;

        public ExpressionAndHelpersCode? ExpressionAndHelpersCode => _expressionAndHelpersCode;
    }
}
