using Fluxx.Api;
using Fluxx.Binding;
using Fluxx.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

namespace Fluxx.Syntax.Expression
{
    public sealed class InvalidExpressionSyntax : ExpressionSyntax
    {
        private readonly TypeBinding typeBinding;
        private readonly string expressionSource;

        public InvalidExpressionSyntax(TextSpan span, string expressionSource, TypeBinding typeBinding) : base(span)
        {
            this.typeBinding = typeBinding;
            this.expressionSource = expressionSource;
        }

        public InvalidExpressionSyntax() : base(TextSpanExtensions.NullTextSpan)
        {
            this.typeBinding = InvalidTypeBinding.Instance;
            this.expressionSource = "<invalid-expression>";
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.InvalidExpression;

        public override void WriteSource(SourceWriter sourceWriter)
        {
            sourceWriter.Write(this.expressionSource);
        }

        public override TypeBinding GetTypeBinding()
        {
            return this.typeBinding;
        }
    }
}
