using Fluxx.Syntax;

namespace Fluxx.Binding.Internal
{
    public class ParameterBinding : SymbolBinding
    {
        private readonly FunctionDefinitionSyntax functionDefinition;
        private readonly int parameterIndex;
        private readonly TypeBinding typeBinding;

        public ParameterBinding(FunctionDefinitionSyntax functionDefinition, int parameterIndex)
        {
            this.functionDefinition = functionDefinition;
            this.parameterIndex = parameterIndex;
            this.typeBinding = functionDefinition.Parameters[parameterIndex].TypeReferenceSyntax.GetTypeBinding();
        }

        public FunctionDefinitionSyntax FunctionDefinition => this.functionDefinition;

        public int ParameterIndex => this.parameterIndex;

        public override TypeBinding GetTypeBinding()
        {
            return this.typeBinding;
        }
    }
}
