namespace Faml.Binding
{
    public abstract class EnumValueBinding
    {
        private readonly EnumTypeBinding enumTypeBinding;

        protected EnumValueBinding(EnumTypeBinding enumTypeBinding)
        {
            this.enumTypeBinding = enumTypeBinding;
        }

        public EnumTypeBinding EnumTypeBinding => this.enumTypeBinding;

        public abstract string Name { get; }
    }
}
