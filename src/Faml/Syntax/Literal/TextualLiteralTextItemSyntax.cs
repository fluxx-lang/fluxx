using Faml.CodeAnalysis.Text;
using Microsoft.CodeAnalysisP.Text;

namespace Faml.Syntax.Literal {
    public sealed class TextualLiteralTextItemSyntax : TextualLiteralItemSyntax {
        private readonly string _text;

        public TextualLiteralTextItemSyntax(TextSpan span, string text) : base(span) {
            _text = text;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.TextualLiteralTextItem;

        public string Text => _text;

        public override void WriteSource(SourceWriter sourceWriter) {
            sourceWriter.Write(_text);
        }
    }
}
