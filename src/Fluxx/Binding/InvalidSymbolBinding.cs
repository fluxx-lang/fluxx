namespace Faml.Binding
{
    public class InvalidSymbolBinding : SymbolBinding
    {
        public static InvalidSymbolBinding Instance = new InvalidSymbolBinding();

        private InvalidSymbolBinding() {}

        public override TypeBinding GetTypeBinding()
        {
            return InvalidTypeBinding.Instance;
        }
    }
}
