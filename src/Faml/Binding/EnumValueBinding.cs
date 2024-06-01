namespace Faml.Binding {
    public abstract class EnumValueBinding {
        private readonly EnumTypeBinding _enumTypeBinding;

        protected EnumValueBinding(EnumTypeBinding enumTypeBinding) {
            _enumTypeBinding = enumTypeBinding;
        }

        public EnumTypeBinding EnumTypeBinding => _enumTypeBinding;

        public abstract string Name { get; }
    }
}
