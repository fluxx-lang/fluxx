/**
 * @author Bret Johnson
 * @since 4/15/2015
 */

namespace Faml.Binding
{
    public class FunctionSymbolBinding : SymbolBinding
    {
        private readonly FunctionBinding functionBinding;
        private readonly FunctionBindingFunctionTypeBinding typeBinding;

        public FunctionSymbolBinding(FunctionBinding functionBinding)
        {
            this.functionBinding = functionBinding;
            this.typeBinding = new FunctionBindingFunctionTypeBinding(functionBinding);
        }

        public FunctionBinding FunctionBinding => this.functionBinding;

        public override TypeBinding GetTypeBinding() => this.typeBinding;
    }
}
