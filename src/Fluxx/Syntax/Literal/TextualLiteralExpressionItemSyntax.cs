using Faml.CodeAnalysis.Text;
using Faml.Syntax.Expression;

namespace Faml.Syntax.Literal
{
    public sealed class TextualLiteralExpressionItemSyntax : TextualLiteralItemSyntax
    {
        private readonly ExpressionSyntax expression;

        public TextualLiteralExpressionItemSyntax(ExpressionSyntax expression) : base(expression.Span)
        {
            this.expression = expression;
        }

        public override bool IsTerminalNode() => false;

        public override SyntaxNodeType NodeType => SyntaxNodeType.TextualLiteralExpressionItem;

        public override void VisitChildren(SyntaxVisitor visitor)
        {
            visitor(this.expression);
        }

        public override void WriteSource(SourceWriter sourceWriter)
        {
            sourceWriter.Write(this.expression);
        }
    }
}
