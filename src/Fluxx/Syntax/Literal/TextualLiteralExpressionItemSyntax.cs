using Faml.CodeAnalysis.Text;
using Faml.Syntax.Expression;

namespace Faml.Syntax.Literal {
    public sealed class TextualLiteralExpressionItemSyntax : TextualLiteralItemSyntax {
        private readonly ExpressionSyntax _expression;

        public TextualLiteralExpressionItemSyntax(ExpressionSyntax expression) : base(expression.Span) {
            _expression = expression;
        }

        public override bool IsTerminalNode() => false;

        public override SyntaxNodeType NodeType => SyntaxNodeType.TextualLiteralExpressionItem;

        public override void VisitChildren(SyntaxVisitor visitor) {
            visitor(_expression);
        }

        public override void WriteSource(SourceWriter sourceWriter) {
            sourceWriter.Write(_expression);
        }
    }
}
