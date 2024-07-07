/**
 * @author Bret Johnson
 * @since 6/29/2014 2:15 AM
 */
using Faml.Binding;
using Faml.Binding.Resolver;
using Faml.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax.Expression
{
    public sealed class ForExpressionSyntax : ExpressionSyntax
    {
        private TypeBinding typeBinding = null;
        private readonly ExpressionSyntax expression;
        private readonly ForVariableDefinitionSyntax forVariableDefinition;

        public ForExpressionSyntax(TextSpan span, ExpressionSyntax expression, ForVariableDefinitionSyntax forVariableDefinition) : base(span)
        {
            this.expression = expression;
            expression.SetParent(this);

            this.forVariableDefinition = forVariableDefinition;
            this.forVariableDefinition.SetParent(this);
        }

        public override bool IsTerminalNode()
        {
            return false;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.ForExpression;

        public override void VisitChildren(SyntaxNode.SyntaxVisitor visitor)
        {
            // Visit in this order so that the variable type binding is resolved before the expression
            // type binding is resolved

            visitor(this.forVariableDefinition);
            visitor(this.expression);
        }

        protected internal override void ResolveBindings(BindingResolver bindingResolver)
        {
            TypeBinding expressionTypeBinding = this.expression.GetTypeBinding();
            this.typeBinding = new SequenceTypeBinding(expressionTypeBinding);
        }

        public override TypeBinding GetTypeBinding() => this.typeBinding;

        public override void WriteSource(SourceWriter sourceWriter)
        {
            this.expression.WriteSource(sourceWriter);
            sourceWriter.Write("for ");
            sourceWriter.Write(this.forVariableDefinition);
        }

        public ExpressionSyntax Expression => this.expression;

        public ForVariableDefinitionSyntax ForVariableDefinition => this.forVariableDefinition;
    }
}
