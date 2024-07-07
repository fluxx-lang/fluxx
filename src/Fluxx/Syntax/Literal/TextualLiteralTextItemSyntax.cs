using Faml.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax.Literal {
    public sealed class TextualLiteralTextItemSyntax : TextualLiteralItemSyntax {
        private readonly string _text;

        public TextualLiteralTextItemSyntax(TextSpan span, string text) : base(span) {
            this._text = text;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.TextualLiteralTextItem;

        public string Text => this._text;

        public override void WriteSource(SourceWriter sourceWriter) {
            sourceWriter.Write(this._text);
        }
    }
}
