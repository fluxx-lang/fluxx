/**
 * @author Bret Johnson
 * @since 6/29/2014 2:15 AM
 */

using Faml.Binding;
using Faml.Binding.Resolver;
using Faml.CodeAnalysis.Text;
using Faml.Syntax.Operator;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax.Expression {
    public sealed class InfixExpressionSyntax : ExpressionSyntax {
        private TypeBinding? _typeBinding = null;
        private readonly ExpressionSyntax _leftOperand;
        private readonly InfixOperator _infixOperator;
        private readonly ExpressionSyntax _rightOperand;

        public InfixExpressionSyntax(TextSpan span, ExpressionSyntax leftOperand, InfixOperator infixOperator,
            ExpressionSyntax rightOperand) : base(span) {
            _leftOperand = leftOperand;
            leftOperand.SetParent(this);

            _infixOperator = infixOperator;

            _rightOperand = rightOperand;
            rightOperand.SetParent(this);
        }

        public override bool IsTerminalNode() {
            return false;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.InfixExpression;

        public override void VisitChildren(SyntaxNode.SyntaxVisitor visitor) {
            visitor(_leftOperand);
            visitor(_rightOperand);
        }

        protected internal override void ResolveBindings(BindingResolver bindingResolver) {
            TypeBinding leftOperandTypeBinding = _leftOperand.GetTypeBinding();
            TypeBinding rightOperandTypeBinding = _rightOperand.GetTypeBinding();

            if (!leftOperandTypeBinding.IsValid() || !rightOperandTypeBinding.IsValid())
                _typeBinding = InvalidTypeBinding.Instance;
            else if (!leftOperandTypeBinding.Equals(rightOperandTypeBinding)) {
                AddError(
                    $"Operand types not the same: {leftOperandTypeBinding.TypeName} and {rightOperandTypeBinding.TypeName}");
                _typeBinding = InvalidTypeBinding.Instance;
            }
            else _typeBinding = leftOperandTypeBinding;
        }

        public override TypeBinding GetTypeBinding() {
            return _typeBinding;
        }

        public override void WriteSource(SourceWriter sourceWriter) {
            _leftOperand.WriteSource(sourceWriter);
            sourceWriter.Write(" ");
            sourceWriter.Write(_infixOperator.GetSourceRepresentation());
            sourceWriter.Write(" ");
            _rightOperand.WriteSource(sourceWriter);
        }

        public ExpressionSyntax LeftOperand => _leftOperand;

        public InfixOperator Operator => _infixOperator;

        public ExpressionSyntax RightOperand => _rightOperand;
    }
}
