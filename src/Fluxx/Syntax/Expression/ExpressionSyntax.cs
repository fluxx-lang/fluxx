using Microsoft.CodeAnalysis.Text;

namespace Fluxx.Syntax.Expression
{
    public abstract class ExpressionSyntax : SyntaxNode
    {
        protected ExpressionSyntax(TextSpan span) : base(span) {}

        public abstract Binding.TypeBinding GetTypeBinding();
    }
}
