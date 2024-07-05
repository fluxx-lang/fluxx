
/**
 * @author Bret Johnson
 * @since 4/15/2015
 */

using Faml.Syntax;

namespace Faml.Binding.Internal {
    public class ParameterBinding : SymbolBinding {
        private readonly FunctionDefinitionSyntax _functionDefinition;
        private readonly int _parameterIndex;
        private readonly TypeBinding _typeBinding;

        public ParameterBinding(FunctionDefinitionSyntax functionDefinition, int parameterIndex) {
            _functionDefinition = functionDefinition;
            _parameterIndex = parameterIndex;
            _typeBinding = functionDefinition.Parameters[parameterIndex].TypeReferenceSyntax.GetTypeBinding();
        }

        public FunctionDefinitionSyntax FunctionDefinition => _functionDefinition;

        public int ParameterIndex => _parameterIndex;

        public override TypeBinding GetTypeBinding() {
            return _typeBinding;
        }
    }
}
