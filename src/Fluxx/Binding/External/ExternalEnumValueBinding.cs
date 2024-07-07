using TypeTooling.Types;


namespace Faml.Binding.External {
    public class ExternalEnumValueBinding : EnumValueBinding {
        private readonly EnumValue _value;

        public ExternalEnumValueBinding(ExternalEnumTypeBinding enumTypeBinding, EnumValue value) : base(enumTypeBinding) {
            this._value = value;
        }

        public override string Name => this._value.Name;

        public EnumValue EnumValue => this._value;
    }
}
