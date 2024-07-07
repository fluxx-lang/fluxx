

/**
 * Created by Bret on 6/28/2014.
 */

using Faml.Binding.Resolver;
using Faml.CodeAnalysis.Text;
using Faml.Parser;
using Faml.Syntax.Expression;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax
{
    public sealed class ConditionValuePairSyntax : SyntaxNode
    {
        private readonly ExpressionSyntax condition;
        private readonly TextSpan valueSpan;
        private ExpressionSyntax value;

        public ConditionValuePairSyntax(TextSpan span, Expression.ExpressionSyntax condition, TextSpan valueSpan) : base(span)
        {
            this.condition = condition;
            this.condition.SetParent(this);

            this.valueSpan = valueSpan;
        }

        public ExpressionSyntax Condition => this.condition;

        public ExpressionSyntax Value => this.value;

        public void ParseValueSource(BindingResolver bindingResolver)
        {
            if (this.value != null)
            {
                return;
            }

            this.value = SourceParser.ParseTextBlockExpression(this.GetModule(), this.valueSpan);
            this.value.SetParent(this);

            // Now resolve the bindings on what we just parsed, since the parse was in turn triggered by resolving bindings
            this.value.VisitNodeAndDescendentsPostorder((astNode) => { astNode.ResolveBindings(bindingResolver); });
        }

        public override void VisitChildren(SyntaxVisitor visitor)
        {
            visitor(this.condition);

            if (this.value != null)
            {
                visitor(this.value);
            }
        }

        public override bool IsTerminalNode() => false;

        public override SyntaxNodeType NodeType => SyntaxNodeType.ArgumentNameValuePair;

        public override void WriteSource(SourceWriter sourceWriter)
        {
            sourceWriter.Write("|");
            sourceWriter.Write(this.Condition);
            sourceWriter.Write(": ");

            sourceWriter.Write(this.Value);
        }
    }
}
