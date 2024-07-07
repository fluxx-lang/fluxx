/**
 * @author Bret Johnson
 * @since 4/15/2015
 */

using Faml.Syntax.Expression;

namespace Faml.Binding.Internal
{
    public class ForSymbolBinding : SymbolBinding
    {
        private readonly ForExpressionSyntax _forExpression;
        private readonly int _variableIndex;
        private readonly TypeBinding _typeBinding;

        public ForSymbolBinding(ForExpressionSyntax forExpression, int variableIndex)
        {
            this._forExpression = forExpression;
            this._variableIndex = variableIndex;
            this._typeBinding = forExpression.ForVariableDefinition.GetVariableTypeBinding();
        }

        public ForExpressionSyntax ForExpression => this._forExpression;

        public int VariableIndex => this._variableIndex;

        public override TypeBinding GetTypeBinding()
        {
            return this._typeBinding;
        }
    }
}
