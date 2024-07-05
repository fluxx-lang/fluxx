/**
 * @author Bret Johnson
 * @since 4/15/2015
 */

namespace Faml.Binding
{
    public class FunctionSymbolBinding : SymbolBinding {
        private readonly FunctionBinding _functionBinding;
        private readonly FunctionBindingFunctionTypeBinding _typeBinding;

        public FunctionSymbolBinding(FunctionBinding functionBinding) {
            _functionBinding = functionBinding;
            _typeBinding = new FunctionBindingFunctionTypeBinding(functionBinding);
        }

        public FunctionBinding FunctionBinding => _functionBinding;

        public override TypeBinding GetTypeBinding() => _typeBinding;
    }
}
