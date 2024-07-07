using Fluxx.Syntax.Expression;

namespace Fluxx.Binding.Internal
{
    public class ForSymbolBinding : SymbolBinding
    {
        private readonly ForExpressionSyntax forExpression;
        private readonly int variableIndex;
        private readonly TypeBinding typeBinding;

        public ForSymbolBinding(ForExpressionSyntax forExpression, int variableIndex)
        {
            this.forExpression = forExpression;
            this.variableIndex = variableIndex;
            this.typeBinding = forExpression.ForVariableDefinition.GetVariableTypeBinding();
        }

        public ForExpressionSyntax ForExpression => this.forExpression;

        public int VariableIndex => this.variableIndex;

        public override TypeBinding GetTypeBinding()
        {
            return this.typeBinding;
        }
    }
}
