/**
 * @author Bret Johnson
 * @since 6/29/2014 2:15 AM
 */
using System.Linq;
using Fluxx.Binding;
using Fluxx.Binding.Resolver;
using Fluxx.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

namespace Fluxx.Syntax.Expression
{
    public sealed class SequenceLiteralExpressionSyntax : ExpressionSyntax
    {
        private SequenceTypeBinding typeBinding;
        private readonly ExpressionSyntax[] expressions;

        // TODO: ADD AST structure properties

        public SequenceLiteralExpressionSyntax(TextSpan span, ExpressionSyntax[] expressions) : base(span)
        {
            this.expressions = expressions;
            foreach (ExpressionSyntax expression in expressions)
            {
                expression.SetParent(this);
            }
        }

        public ExpressionSyntax[] Expressions => this.expressions;

        public override bool IsTerminalNode() { return false; }

        public override SyntaxNodeType NodeType => SyntaxNodeType.SequenceExpression;

        public override void VisitChildren(SyntaxVisitor visitor)
        {
            foreach (ExpressionSyntax expression in this.expressions)
            {
                visitor(expression);
            }
        }

        protected internal override void ResolveBindings(BindingResolver bindingResolver)
        {
            if (this.expressions.Length > 0)
            {
                TypeBinding elementTypeBinding = TypeUtil.FindCommonType(this.expressions.Select(e => e.GetTypeBinding()));

                if (elementTypeBinding == null)
                {
                    this.AddError("Couldn't find a common type for elements of sequence");
                    this.typeBinding = new SequenceTypeBinding(InvalidTypeBinding.Instance);
                }
                else
                {
                    this.typeBinding = new SequenceTypeBinding(elementTypeBinding);
                }
            }
            else
            {
                // TODO: Handle this better
                this.AddError("Empty lists aren't currently supported");
                this.typeBinding = new SequenceTypeBinding(InvalidTypeBinding.Instance);
            }
        }

        public override TypeBinding GetTypeBinding()
        {
            return this.typeBinding;
        }

        public override void WriteSource(SourceWriter sourceWriter)
        {
            foreach (ExpressionSyntax expression in this.expressions)
            {
                expression.WriteSource(sourceWriter);
                sourceWriter.Writeln();
            }
        }
    }
}
