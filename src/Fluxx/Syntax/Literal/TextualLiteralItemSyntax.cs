using Microsoft.CodeAnalysisP.Text;

namespace Faml.Syntax.Literal {
    public abstract class TextualLiteralItemSyntax : SyntaxNode {
        protected TextualLiteralItemSyntax(TextSpan span) : 
            base(span) { }
    }
}
