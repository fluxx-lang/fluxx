using Faml.CodeAnalysis.Text;
using Faml.Syntax.Expression;
using Microsoft.CodeAnalysis.Text;

/**
 * @author Bret Johnson
 * @since 4/12/2015
 */

namespace Faml.Syntax {
    public class ExampleDefinitionSyntax : SyntaxNode {
        private readonly ExpressionSyntax _expression;
        private int _exampleIndex;

        public ExampleDefinitionSyntax(TextSpan span, ExpressionSyntax expression) : base(span) {
            _expression = expression;

            _expression.SetParent(this);
        }

        public ExpressionSyntax Expression => _expression;

        internal void SetExampleIndex(int exampleIndex) {
            _exampleIndex = exampleIndex;
        }

        public int ExampleIndex => _exampleIndex;

        public override bool IsTerminalNode() {
            return false;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.ExampleDefinition;

        public override void VisitChildren(SyntaxVisitor visitor) {
            visitor(_expression);
        }

        public override void WriteSource(SourceWriter sourceWriter) {
            sourceWriter.Write(_expression);
        }
    }
}
