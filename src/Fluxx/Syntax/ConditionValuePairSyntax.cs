

/**
 * Created by Bret on 6/28/2014.
 */

using Faml.Binding.Resolver;
using Faml.CodeAnalysis.Text;
using Faml.Parser;
using Faml.Syntax.Expression;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax {
    public sealed class ConditionValuePairSyntax : SyntaxNode {
        private readonly ExpressionSyntax _condition;
        private readonly TextSpan _valueSpan;
        private ExpressionSyntax _value;

        public ConditionValuePairSyntax(TextSpan span, Expression.ExpressionSyntax condition, TextSpan valueSpan) : base(span) {
            this._condition = condition;
            this._condition.SetParent(this);

            this._valueSpan = valueSpan;
        }

        public ExpressionSyntax Condition => this._condition;

        public ExpressionSyntax Value => this._value;

        public void ParseValueSource(BindingResolver bindingResolver) {
            if (this._value != null)
                return;

            this._value = SourceParser.ParseTextBlockExpression(this.GetModule(), this._valueSpan);
            this._value.SetParent(this);

            // Now resolve the bindings on what we just parsed, since the parse was in turn triggered by resolving bindings
            this._value.VisitNodeAndDescendentsPostorder((astNode) => { astNode.ResolveBindings(bindingResolver); });
        }

        public override void VisitChildren(SyntaxVisitor visitor) {
            visitor(this._condition);

            if (this._value != null)
                visitor(this._value);
        }

        public override bool IsTerminalNode() => false;

        public override SyntaxNodeType NodeType => SyntaxNodeType.ArgumentNameValuePair;

        public override void WriteSource(SourceWriter sourceWriter) {
            sourceWriter.Write("|");
            sourceWriter.Write(this.Condition);
            sourceWriter.Write(": ");

            sourceWriter.Write(this.Value);
        }
    }
}
