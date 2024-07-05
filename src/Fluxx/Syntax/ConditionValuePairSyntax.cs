

/**
 * Created by Bret on 6/28/2014.
 */

using Microsoft.CodeAnalysisP.Text;
using Faml.Binding.Resolver;
using Faml.Parser;
using Faml.Syntax.Expression;
using Faml.CodeAnalysis.Text;

namespace Faml.Syntax {
    public sealed class ConditionValuePairSyntax : SyntaxNode {
        private readonly ExpressionSyntax _condition;
        private readonly TextSpan _valueSpan;
        private ExpressionSyntax _value;

        public ConditionValuePairSyntax(TextSpan span, Expression.ExpressionSyntax condition, TextSpan valueSpan) : base(span) {
            _condition = condition;
            _condition.SetParent(this);

            _valueSpan = valueSpan;
        }

        public ExpressionSyntax Condition => _condition;

        public ExpressionSyntax Value => _value;

        public void ParseValueSource(BindingResolver bindingResolver) {
            if (_value != null)
                return;

            _value = SourceParser.ParseTextBlockExpression(GetModule(), _valueSpan);
            _value.SetParent(this);

            // Now resolve the bindings on what we just parsed, since the parse was in turn triggered by resolving bindings
            _value.VisitNodeAndDescendentsPostorder((astNode) => { astNode.ResolveBindings(bindingResolver); });
        }

        public override void VisitChildren(SyntaxVisitor visitor) {
            visitor(_condition);

            if (_value != null)
                visitor(_value);
        }

        public override bool IsTerminalNode() => false;

        public override SyntaxNodeType NodeType => SyntaxNodeType.ArgumentNameValuePair;

        public override void WriteSource(SourceWriter sourceWriter) {
            sourceWriter.Write("|");
            sourceWriter.Write(Condition);
            sourceWriter.Write(": ");

            sourceWriter.Write(Value);
        }
    }
}
