/**
 * @author Bret Johnson
 * @since 6/29/2014 2:16 AM
 */

using Faml.Binding;
using Faml.CodeAnalysis.Text;
using Faml.Syntax.Operator;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax.Expression {
    public sealed class PrefixExpressionSyntax : ExpressionSyntax {
        private readonly PrefixOperator _prefixOperator;
        private readonly ExpressionSyntax _operand;

        // AST structure properties
        public PrefixExpressionSyntax(TextSpan span, PrefixOperator prefixOperator, ExpressionSyntax operand) : base(span) {
            _prefixOperator = prefixOperator;

            _operand = operand;
            operand.SetParent(this);
        }

        public PrefixOperator Operator => _prefixOperator;

        public ExpressionSyntax Operand => _operand;

        public override bool IsTerminalNode() {
            return false;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.PrefixExpression;

        public override void VisitChildren(SyntaxNode.SyntaxVisitor visitor) {
            visitor(_operand);
        }

        public override TypeBinding GetTypeBinding() {
            return _operand.GetTypeBinding();
        }

        public override void WriteSource(SourceWriter sourceWriter) {
            sourceWriter.Write(_prefixOperator.GetSourceRepresentation());
            _operand.WriteSource(sourceWriter);
        }
    }
}
