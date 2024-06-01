using TypeTooling.Types;


namespace Faml.Binding.External {
    public class ExternalEnumValueBinding : EnumValueBinding {
        private readonly EnumValue _value;

        public ExternalEnumValueBinding(ExternalEnumTypeBinding enumTypeBinding, EnumValue value) : base(enumTypeBinding) {
            _value = value;
        }

        public override string Name => _value.Name;

        public EnumValue EnumValue => _value;
    }
}
