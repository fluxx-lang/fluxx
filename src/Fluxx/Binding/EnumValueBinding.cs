namespace Faml.Binding
{
    public abstract class EnumValueBinding
    {
        private readonly EnumTypeBinding _enumTypeBinding;

        protected EnumValueBinding(EnumTypeBinding enumTypeBinding)
        {
            this._enumTypeBinding = enumTypeBinding;
        }

        public EnumTypeBinding EnumTypeBinding => this._enumTypeBinding;

        public abstract string Name { get; }
    }
}
