using Microsoft.CodeAnalysis.Text;

namespace Fluxx.Syntax.Literal
{
    public abstract class TextualLiteralItemSyntax : SyntaxNode
    {
        protected TextualLiteralItemSyntax(TextSpan span) : base(span) { }
    }
}
