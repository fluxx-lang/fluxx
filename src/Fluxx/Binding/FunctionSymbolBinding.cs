/**
 * @author Bret Johnson
 * @since 4/15/2015
 */

namespace Faml.Binding
{
    public class FunctionSymbolBinding : SymbolBinding
    {
        private readonly FunctionBinding _functionBinding;
        private readonly FunctionBindingFunctionTypeBinding _typeBinding;

        public FunctionSymbolBinding(FunctionBinding functionBinding)
        {
            this._functionBinding = functionBinding;
            this._typeBinding = new FunctionBindingFunctionTypeBinding(functionBinding);
        }

        public FunctionBinding FunctionBinding => this._functionBinding;

        public override TypeBinding GetTypeBinding() => this._typeBinding;
    }
}
