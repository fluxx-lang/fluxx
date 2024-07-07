using Faml.Binding;
using Faml.Binding.Resolver;
using Faml.CodeAnalysis.Text;
using Faml.Syntax.Expression;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax
{
    public sealed class ForVariableDefinitionSyntax : SyntaxNode
    {
        private readonly NameSyntax variableNameSyntax;
        private TypeBinding variableTypeBinding = null;
        private readonly ExpressionSyntax inExpression;

        public ForVariableDefinitionSyntax(TextSpan span, NameSyntax variableNameSyntax, ExpressionSyntax inExpression) : base(span)
        {
            this.variableNameSyntax = variableNameSyntax;
            variableNameSyntax.SetParent(this);

            this.inExpression = inExpression;
            inExpression.SetParent(this);
        }

        public NameSyntax VariableNameSyntax => this.variableNameSyntax;

        public ExpressionSyntax InExpression => this.inExpression;

        public override void VisitChildren(SyntaxVisitor visitor)
        {
            visitor(this.inExpression);
            visitor(this.variableNameSyntax);
        }

        protected internal override void ResolveBindings(BindingResolver bindingResolver)
        {
            TypeBinding inExpressionTypeBinding = this.inExpression.GetTypeBinding();
            if (inExpressionTypeBinding == null)
            {
                this.variableNameSyntax.AddError("Couldn't infer variable type from 'in' expression");
            }

            // TODO: Remove this restriction
            if (!(inExpressionTypeBinding is SequenceTypeBinding inExpressionSequenceTypeBinding))
            {
                this.inExpression.AddError("'in' expression isn't a sequence; currently it must have at least two items");
            }
            else
            {
                this.variableTypeBinding = inExpressionSequenceTypeBinding.ElementType;
            }
        }

        public TypeBinding GetVariableTypeBinding() => this.variableTypeBinding;

        public override bool IsTerminalNode()
        {
            return false;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.ForVariableDefinition;

        public override void WriteSource(SourceWriter sourceWriter)
        {
            sourceWriter.Write(this.variableNameSyntax);
            sourceWriter.Write(" in ");
            sourceWriter.Write(this.inExpression);
        }
    }
}
