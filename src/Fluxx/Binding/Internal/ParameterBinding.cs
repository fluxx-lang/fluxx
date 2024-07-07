
/**
 * @author Bret Johnson
 * @since 4/15/2015
 */

using Faml.Syntax;

namespace Faml.Binding.Internal
{
    public class ParameterBinding : SymbolBinding
    {
        private readonly FunctionDefinitionSyntax _functionDefinition;
        private readonly int _parameterIndex;
        private readonly TypeBinding _typeBinding;

        public ParameterBinding(FunctionDefinitionSyntax functionDefinition, int parameterIndex)
        {
            this._functionDefinition = functionDefinition;
            this._parameterIndex = parameterIndex;
            this._typeBinding = functionDefinition.Parameters[parameterIndex].TypeReferenceSyntax.GetTypeBinding();
        }

        public FunctionDefinitionSyntax FunctionDefinition => this._functionDefinition;

        public int ParameterIndex => this._parameterIndex;

        public override TypeBinding GetTypeBinding()
        {
            return this._typeBinding;
        }
    }
}
