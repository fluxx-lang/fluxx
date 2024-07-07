/**
 * @author Bret Johnson
 * @since 6/29/2014 2:15 AM
 */

using Faml.Binding;
using Faml.Binding.Resolver;
using Faml.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax.Expression {
    public sealed class ForExpressionSyntax : ExpressionSyntax {
        private TypeBinding _typeBinding = null;
        private readonly ExpressionSyntax _expression;
        private readonly ForVariableDefinitionSyntax _forVariableDefinition;


        public ForExpressionSyntax(TextSpan span, ExpressionSyntax expression, ForVariableDefinitionSyntax forVariableDefinition) : base(span) {
            _expression = expression;
            expression.SetParent(this);

            _forVariableDefinition = forVariableDefinition;
            _forVariableDefinition.SetParent(this);
        }

        public override bool IsTerminalNode() {
            return false;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.ForExpression;

        public override void VisitChildren(SyntaxNode.SyntaxVisitor visitor) {
            // Visit in this order so that the variable type binding is resolved before the expression
            // type binding is resolved

            visitor(_forVariableDefinition);
            visitor(_expression);
        }

        protected internal override void ResolveBindings(BindingResolver bindingResolver) {
            TypeBinding expressionTypeBinding = _expression.GetTypeBinding();
            _typeBinding = new SequenceTypeBinding(expressionTypeBinding);
        }

        public override TypeBinding GetTypeBinding() => _typeBinding;

        public override void WriteSource(SourceWriter sourceWriter) {
            _expression.WriteSource(sourceWriter);
            sourceWriter.Write("for ");
            sourceWriter.Write(_forVariableDefinition);
        }

        public ExpressionSyntax Expression => _expression;

        public ForVariableDefinitionSyntax ForVariableDefinition => _forVariableDefinition;
    }
}
