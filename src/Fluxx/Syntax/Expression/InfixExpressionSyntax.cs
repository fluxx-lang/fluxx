/**
 * @author Bret Johnson
 * @since 6/29/2014 2:15 AM
 */
using Fluxx.Binding;
using Fluxx.Binding.Resolver;
using Fluxx.CodeAnalysis.Text;
using Fluxx.Syntax.Operator;
using Microsoft.CodeAnalysis.Text;

namespace Fluxx.Syntax.Expression
{
    public sealed class InfixExpressionSyntax : ExpressionSyntax
    {
        private TypeBinding? typeBinding = null;
        private readonly ExpressionSyntax leftOperand;
        private readonly InfixOperator infixOperator;
        private readonly ExpressionSyntax rightOperand;

        public InfixExpressionSyntax(TextSpan span, ExpressionSyntax leftOperand, InfixOperator infixOperator,
            ExpressionSyntax rightOperand) : base(span)
            {
            this.leftOperand = leftOperand;
            leftOperand.SetParent(this);

            this.infixOperator = infixOperator;

            this.rightOperand = rightOperand;
            rightOperand.SetParent(this);
        }

        public override bool IsTerminalNode()
        {
            return false;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.InfixExpression;

        public override void VisitChildren(SyntaxNode.SyntaxVisitor visitor)
        {
            visitor(this.leftOperand);
            visitor(this.rightOperand);
        }

        protected internal override void ResolveBindings(BindingResolver bindingResolver)
        {
            TypeBinding leftOperandTypeBinding = this.leftOperand.GetTypeBinding();
            TypeBinding rightOperandTypeBinding = this.rightOperand.GetTypeBinding();

            if (!leftOperandTypeBinding.IsValid() || !rightOperandTypeBinding.IsValid())
            {
                this.typeBinding = InvalidTypeBinding.Instance;
            }
            else if (!leftOperandTypeBinding.Equals(rightOperandTypeBinding))
            {
                this.AddError(
                    $"Operand types not the same: {leftOperandTypeBinding.TypeName} and {rightOperandTypeBinding.TypeName}");
                this.typeBinding = InvalidTypeBinding.Instance;
            }
            else
            {
                this.typeBinding = leftOperandTypeBinding;
            }
        }

        public override TypeBinding GetTypeBinding()
        {
            return this.typeBinding;
        }

        public override void WriteSource(SourceWriter sourceWriter)
        {
            this.leftOperand.WriteSource(sourceWriter);
            sourceWriter.Write(" ");
            sourceWriter.Write(this.infixOperator.GetSourceRepresentation());
            sourceWriter.Write(" ");
            this.rightOperand.WriteSource(sourceWriter);
        }

        public ExpressionSyntax LeftOperand => this.leftOperand;

        public InfixOperator Operator => this.infixOperator;

        public ExpressionSyntax RightOperand => this.rightOperand;
    }
}
