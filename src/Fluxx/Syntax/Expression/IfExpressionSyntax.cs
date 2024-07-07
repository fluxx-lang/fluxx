using Faml.Binding;
using Faml.Binding.Resolver;
using Faml.CodeAnalysis.Text;
using Faml.Parser;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax.Expression {
    public class IfExpressionSyntax : ExpressionSyntax {
        private readonly ConditionValuePairSyntax[] _conditionValuePairs;
        private readonly TextSpan? _elseSpan;
        private ExpressionSyntax? _elseValue;
        private TypeBinding? _typeBinding;

        public IfExpressionSyntax(TextSpan span, ConditionValuePairSyntax[] conditionValuePairs, TextSpan? elseSpan) : base(span) {
            _conditionValuePairs = conditionValuePairs;
            foreach (ConditionValuePairSyntax conditionValuePair in conditionValuePairs)
                conditionValuePair.SetParent(this);

            _elseSpan = elseSpan;
        }

        public ConditionValuePairSyntax[] ConditionValuePairs => _conditionValuePairs;

        public ExpressionSyntax? ElseValue => _elseValue;

        public override bool IsTerminalNode() {
            return false;
        }

        public override void VisitChildren(SyntaxVisitor visitor) {
            foreach (ConditionValuePairSyntax conditionValuePair in _conditionValuePairs) {
                visitor(conditionValuePair);
            }

            if (_elseValue != null)
                visitor(_elseValue);
        }

        protected internal override void ResolveBindings(BindingResolver bindingResolver) {
            TypeBinding? typeBinding = null;

            foreach (ConditionValuePairSyntax conditionValuePair in _conditionValuePairs) {
                conditionValuePair.ParseValueSource(bindingResolver);

                TypeBinding currentTypeBinding = conditionValuePair.Value.GetTypeBinding();

                if (typeBinding == null)
                    typeBinding = currentTypeBinding;
                else {
                    if (!currentTypeBinding.IsSameAs(typeBinding)) {
                        AddError($"Different conditions of 'if' don't all evaluate to the same type");
                        return;
                    }
                }
            }

            if (_elseSpan.HasValue) {
                if (_elseValue == null) {
                    _elseValue = SourceParser.ParseTextBlockExpression(GetModule(), _elseSpan.Value);
                    _elseValue.SetParent(this);

                    // Now resolve the bindings on what we just parsed
                    _elseValue.VisitNodeAndDescendentsPostorder((astNode) => { astNode.ResolveBindings(bindingResolver); });
                }

                TypeBinding currentTypeBinding = _elseValue.GetTypeBinding();

                if (typeBinding == null)
                    typeBinding = currentTypeBinding;
                else {
                    if (!currentTypeBinding.IsSameAs(typeBinding)) {
                        AddError($"Different conditions of 'if' don't all evaluate to the same type");
                        return;
                    }
                }
            }

            _typeBinding = typeBinding;
        }

        public override TypeBinding GetTypeBinding() {
            return _typeBinding;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.IfExpression;

        public override void WriteSource(SourceWriter sourceWriter) {
            sourceWriter.Writeln("if");

            foreach (ConditionValuePairSyntax conditionValuePair in _conditionValuePairs)
                sourceWriter.Writeln(conditionValuePair);

            if (_elseValue != null) {
                sourceWriter.Write("|: ");
                sourceWriter.Writeln(_elseValue);
            }
        }
    }
}
