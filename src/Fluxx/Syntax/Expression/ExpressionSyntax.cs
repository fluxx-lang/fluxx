using Microsoft.CodeAnalysis.Text;


namespace Faml.Syntax.Expression
{
    public abstract class ExpressionSyntax : SyntaxNode
    {
        protected ExpressionSyntax(TextSpan span) : base(span) {}

        public abstract Binding.TypeBinding GetTypeBinding();
    }
}
