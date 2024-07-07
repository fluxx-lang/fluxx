/**
 * @author Bret Johnson
 * @since 6/29/2014 2:15 AM
 */

using System.Linq;
using Faml.Binding;
using Faml.Binding.Resolver;
using Faml.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax.Expression
{
    public sealed class SequenceLiteralExpressionSyntax : ExpressionSyntax
    {
        private SequenceTypeBinding _typeBinding;
        private readonly ExpressionSyntax[] _expressions;

        // TODO: ADD AST structure properties

        public SequenceLiteralExpressionSyntax(TextSpan span, ExpressionSyntax[] expressions) : base(span)
        {
            this._expressions = expressions;
            foreach (ExpressionSyntax expression in expressions)
            {
                expression.SetParent(this);
            }
        }

        public ExpressionSyntax[] Expressions => this._expressions;

        public override bool IsTerminalNode() { return false; }

        public override SyntaxNodeType NodeType => SyntaxNodeType.SequenceExpression;

        public override void VisitChildren(SyntaxVisitor visitor)
        {
            foreach (ExpressionSyntax expression in this._expressions)
            {
                visitor(expression);
            }
        }

        protected internal override void ResolveBindings(BindingResolver bindingResolver)
        {
            if (this._expressions.Length > 0)
            {
                TypeBinding elementTypeBinding = TypeUtil.FindCommonType(this._expressions.Select(e => e.GetTypeBinding()));

                if (elementTypeBinding == null)
                {
                    this.AddError("Couldn't find a common type for elements of sequence");
                    this._typeBinding = new SequenceTypeBinding(InvalidTypeBinding.Instance);
                }
                else this._typeBinding = new SequenceTypeBinding(elementTypeBinding);
            }
            else
            {
                // TODO: Handle this better
                this.AddError("Empty lists aren't currently supported");
                this._typeBinding = new SequenceTypeBinding(InvalidTypeBinding.Instance);
            }
        }

        public override TypeBinding GetTypeBinding()
        {
            return this._typeBinding;
        }

        public override void WriteSource(SourceWriter sourceWriter)
        {
            foreach (ExpressionSyntax expression in this._expressions)
            {
                expression.WriteSource(sourceWriter);
                sourceWriter.Writeln();
            }
        }
    }
}
