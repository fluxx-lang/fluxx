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
            this._expression = expression;
            expression.SetParent(this);

            this._forVariableDefinition = forVariableDefinition;
            this._forVariableDefinition.SetParent(this);
        }

        public override bool IsTerminalNode() {
            return false;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.ForExpression;

        public override void VisitChildren(SyntaxNode.SyntaxVisitor visitor) {
            // Visit in this order so that the variable type binding is resolved before the expression
            // type binding is resolved

            visitor(this._forVariableDefinition);
            visitor(this._expression);
        }

        protected internal override void ResolveBindings(BindingResolver bindingResolver) {
            TypeBinding expressionTypeBinding = this._expression.GetTypeBinding();
            this._typeBinding = new SequenceTypeBinding(expressionTypeBinding);
        }

        public override TypeBinding GetTypeBinding() => this._typeBinding;

        public override void WriteSource(SourceWriter sourceWriter) {
            this._expression.WriteSource(sourceWriter);
            sourceWriter.Write("for ");
            sourceWriter.Write(this._forVariableDefinition);
        }

        public ExpressionSyntax Expression => this._expression;

        public ForVariableDefinitionSyntax ForVariableDefinition => this._forVariableDefinition;
    }
}
