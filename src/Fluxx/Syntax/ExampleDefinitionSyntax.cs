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
            this._expression = expression;

            this._expression.SetParent(this);
        }

        public ExpressionSyntax Expression => this._expression;

        internal void SetExampleIndex(int exampleIndex) {
            this._exampleIndex = exampleIndex;
        }

        public int ExampleIndex => this._exampleIndex;

        public override bool IsTerminalNode() {
            return false;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.ExampleDefinition;

        public override void VisitChildren(SyntaxVisitor visitor) {
            visitor(this._expression);
        }

        public override void WriteSource(SourceWriter sourceWriter) {
            sourceWriter.Write(this._expression);
        }
    }
}
