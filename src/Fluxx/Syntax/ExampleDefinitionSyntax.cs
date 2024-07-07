using Fluxx.CodeAnalysis.Text;
using Fluxx.Syntax.Expression;
using Microsoft.CodeAnalysis.Text;

/**
 * @author Bret Johnson
 * @since 4/12/2015
 */
namespace Fluxx.Syntax
{
    public class ExampleDefinitionSyntax : SyntaxNode
    {
        private readonly ExpressionSyntax expression;
        private int exampleIndex;

        public ExampleDefinitionSyntax(TextSpan span, ExpressionSyntax expression) : base(span)
        {
            this.expression = expression;

            this.expression.SetParent(this);
        }

        public ExpressionSyntax Expression => this.expression;

        internal void SetExampleIndex(int exampleIndex)
        {
            this.exampleIndex = exampleIndex;
        }

        public int ExampleIndex => this.exampleIndex;

        public override bool IsTerminalNode()
        {
            return false;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.ExampleDefinition;

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
