
/**
 * @author Bret Johnson
 * @since 6/29/2014 5:18 PM
 */
using Fluxx.Binding;
using Fluxx.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

namespace Fluxx.Syntax.Expression
{
    public sealed class BracedExpressionSyntax : ExpressionSyntax
    {
        private readonly ExpressionSyntax expression;
        //private @Nullable Type type;

        public BracedExpressionSyntax(TextSpan span, ExpressionSyntax expression) : base(span)
        {
            this.expression = expression;
            expression.SetParent(this);
        }

        public override bool IsTerminalNode() { return false; }
        public override SyntaxNodeType NodeType => SyntaxNodeType.BracedExpression;

        public override void VisitChildren(SyntaxNode.SyntaxVisitor visitor)
        {
            visitor(this.expression);
        }

        public override TypeBinding GetTypeBinding()
        {
            return this.expression.GetTypeBinding();
        }

        public override void WriteSource(SourceWriter sourceWriter)
        {
            sourceWriter.Write("{ ");
            this.expression.WriteSource(sourceWriter);
            sourceWriter.Write(" }");
        }

        public ExpressionSyntax Expression => this.expression;
    }
}
