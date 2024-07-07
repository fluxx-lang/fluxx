/**
 * @author Bret Johnson
 * @since 6/29/2014 2:16 AM
 */

using Faml.Binding;
using Faml.CodeAnalysis.Text;
using Faml.Syntax.Operator;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax.Expression
{
    public sealed class PrefixExpressionSyntax : ExpressionSyntax
    {
        private readonly PrefixOperator prefixOperator;
        private readonly ExpressionSyntax operand;

        // AST structure properties
        public PrefixExpressionSyntax(TextSpan span, PrefixOperator prefixOperator, ExpressionSyntax operand) : base(span)
        {
            this.prefixOperator = prefixOperator;

            this.operand = operand;
            operand.SetParent(this);
        }

        public PrefixOperator Operator => this.prefixOperator;

        public ExpressionSyntax Operand => this.operand;

        public override bool IsTerminalNode()
        {
            return false;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.PrefixExpression;

        public override void VisitChildren(SyntaxNode.SyntaxVisitor visitor)
        {
            visitor(this.operand);
        }

        public override TypeBinding GetTypeBinding()
        {
            return this.operand.GetTypeBinding();
        }

        public override void WriteSource(SourceWriter sourceWriter)
        {
            sourceWriter.Write(this.prefixOperator.GetSourceRepresentation());
            this.operand.WriteSource(sourceWriter);
        }
    }
}
