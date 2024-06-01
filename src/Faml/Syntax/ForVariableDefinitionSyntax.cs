using Microsoft.CodeAnalysisP.Text;
using Faml.Binding;
using Faml.Binding.Resolver;
using Faml.CodeAnalysis.Text;
using Faml.Syntax.Expression;

/**
 * Created by Bret on 6/28/2014.
 */
namespace Faml.Syntax {
    public sealed class ForVariableDefinitionSyntax : SyntaxNode {
        private readonly NameSyntax _variableNameSyntax;
        private TypeBinding _variableTypeBinding = null;
        private readonly ExpressionSyntax _inExpression;

        public ForVariableDefinitionSyntax(TextSpan span, NameSyntax variableNameSyntax, ExpressionSyntax inExpression) : base(span) {
            _variableNameSyntax = variableNameSyntax;
            variableNameSyntax.SetParent(this);

            _inExpression = inExpression;
            inExpression.SetParent(this);
        }

        public NameSyntax VariableNameSyntax => _variableNameSyntax;

        public ExpressionSyntax InExpression => _inExpression;

        public override void VisitChildren(SyntaxVisitor visitor) {
            visitor(_inExpression);
            visitor(_variableNameSyntax);
        }

        protected internal override void ResolveBindings(BindingResolver bindingResolver) {
            TypeBinding inExpressionTypeBinding = _inExpression.GetTypeBinding();
            if (inExpressionTypeBinding == null)
                _variableNameSyntax.AddError("Couldn't infer variable type from 'in' expression");

            // TODO: Remove this restriction
            if (!(inExpressionTypeBinding is SequenceTypeBinding inExpressionSequenceTypeBinding))
                _inExpression.AddError("'in' expression isn't a sequence; currently it must have at least two items");
            else _variableTypeBinding = inExpressionSequenceTypeBinding.ElementType;
        }

        public TypeBinding GetVariableTypeBinding () => _variableTypeBinding;

        public override bool IsTerminalNode() {
            return false;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.ForVariableDefinition;

        public override void WriteSource(SourceWriter sourceWriter) {
            sourceWriter.Write(_variableNameSyntax);
            sourceWriter.Write(" in ");
            sourceWriter.Write(_inExpression);
        }
    }
}
