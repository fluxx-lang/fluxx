using Faml.Api;
using Faml.Binding;
using Faml.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

/**
 * Created by Bret on 6/28/2014.
 */
namespace Faml.Syntax.Expression {
    public sealed class InvalidExpressionSyntax : ExpressionSyntax {
        private readonly TypeBinding _typeBinding;
        private readonly string _expressionSource;

        public InvalidExpressionSyntax(TextSpan span, string expressionSource, TypeBinding typeBinding) : base(span) {
            this._typeBinding = typeBinding;
            this._expressionSource = expressionSource;
        }

        public InvalidExpressionSyntax() : base(TextSpanExtensions.NullTextSpan) {
            this._typeBinding = InvalidTypeBinding.Instance;
            this._expressionSource = "<invalid-expression>";
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.InvalidExpression;

        public override void WriteSource(SourceWriter sourceWriter) {
            sourceWriter.Write(this._expressionSource);
        }

        public override TypeBinding GetTypeBinding() {
            return this._typeBinding;
        }
    }
}
