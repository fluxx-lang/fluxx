
/**
 * @author Bret Johnson
 * @since 6/29/2014 5:18 PM
 */

using Microsoft.CodeAnalysisP.Text;
using Faml.Binding;
using Faml.CodeAnalysis.Text;

namespace Faml.Syntax.Expression {
    public sealed class BracedExpressionSyntax : ExpressionSyntax {
        private readonly ExpressionSyntax _expression;
        //private @Nullable Type type;

        public BracedExpressionSyntax(TextSpan span, ExpressionSyntax expression) : base(span) {
            _expression = expression;
            expression.SetParent(this);
        }

        public override bool IsTerminalNode() { return false; }
        public override SyntaxNodeType NodeType => SyntaxNodeType.BracedExpression;

        public override void VisitChildren(SyntaxNode.SyntaxVisitor visitor) {
            visitor(_expression);
        }

        public override TypeBinding GetTypeBinding() {
            return _expression.GetTypeBinding();
        }

        public override void WriteSource(SourceWriter sourceWriter) {
            sourceWriter.Write("{ ");
            _expression.WriteSource(sourceWriter);
            sourceWriter.Write(" }");
        }

        public ExpressionSyntax Expression => _expression;
    }
}
