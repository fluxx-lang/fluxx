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
            this._leftOperand = leftOperand;
            leftOperand.SetParent(this);

            this._infixOperator = infixOperator;

            this._rightOperand = rightOperand;
            rightOperand.SetParent(this);
        }

        public override bool IsTerminalNode() {
            return false;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.InfixExpression;

        public override void VisitChildren(SyntaxNode.SyntaxVisitor visitor) {
            visitor(this._leftOperand);
            visitor(this._rightOperand);
        }

        protected internal override void ResolveBindings(BindingResolver bindingResolver) {
            TypeBinding leftOperandTypeBinding = this._leftOperand.GetTypeBinding();
            TypeBinding rightOperandTypeBinding = this._rightOperand.GetTypeBinding();

            if (!leftOperandTypeBinding.IsValid() || !rightOperandTypeBinding.IsValid())
                this._typeBinding = InvalidTypeBinding.Instance;
            else if (!leftOperandTypeBinding.Equals(rightOperandTypeBinding)) {
                this.AddError(
                    $"Operand types not the same: {leftOperandTypeBinding.TypeName} and {rightOperandTypeBinding.TypeName}");
                this._typeBinding = InvalidTypeBinding.Instance;
            }
            else this._typeBinding = leftOperandTypeBinding;
        }

        public override TypeBinding GetTypeBinding() {
            return this._typeBinding;
        }

        public override void WriteSource(SourceWriter sourceWriter) {
            this._leftOperand.WriteSource(sourceWriter);
            sourceWriter.Write(" ");
            sourceWriter.Write(this._infixOperator.GetSourceRepresentation());
            sourceWriter.Write(" ");
            this._rightOperand.WriteSource(sourceWriter);
        }

        public ExpressionSyntax LeftOperand => this._leftOperand;

        public InfixOperator Operator => this._infixOperator;

        public ExpressionSyntax RightOperand => this._rightOperand;
    }
}
