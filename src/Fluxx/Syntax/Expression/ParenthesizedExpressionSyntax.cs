
/**
 * @author Bret Johnson
 * @since 6/29/2014 5:18 PM
 */

using Faml.Binding;
using Faml.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax.Expression
{
    public sealed class ParenthesizedExpressionSyntax : ExpressionSyntax
    {
        private readonly ExpressionSyntax expression;
        //private @Nullable Type type;

        // AST structure properties
        public ParenthesizedExpressionSyntax(TextSpan span, ExpressionSyntax expression) : base(span)
        {
            this.expression = expression;
            expression.SetParent(this);
        }

        public override bool IsTerminalNode()
        {
            return false;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.ParenthesizedExpression;

        public override void VisitChildren(SyntaxVisitor visitor)
        {
            visitor(this.expression);
        }

        public override TypeBinding GetTypeBinding()
        {
            return this.expression.GetTypeBinding();
        }

        public override void WriteSource(SourceWriter sourceWriter)
        {
            sourceWriter.Write("(");
            this.expression.WriteSource(sourceWriter);
            sourceWriter.Write(")");
        }

        public ExpressionSyntax Expression => this.expression;
    }

}
