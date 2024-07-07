using Faml.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax.Literal
{
    public sealed class TextualLiteralTextItemSyntax : TextualLiteralItemSyntax
    {
        private readonly string text;

        public TextualLiteralTextItemSyntax(TextSpan span, string text) : base(span)
        {
            this.text = text;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.TextualLiteralTextItem;

        public string Text => this.text;

        public override void WriteSource(SourceWriter sourceWriter)
        {
            sourceWriter.Write(this.text);
        }
    }
}
