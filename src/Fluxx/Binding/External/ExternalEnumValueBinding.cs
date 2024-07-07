using TypeTooling.Types;


namespace Faml.Binding.External
{
    public class ExternalEnumValueBinding : EnumValueBinding
    {
        private readonly EnumValue value;

        public ExternalEnumValueBinding(ExternalEnumTypeBinding enumTypeBinding, EnumValue value) : base(enumTypeBinding)
        {
            this.value = value;
        }

        public override string Name => this.value.Name;

        public EnumValue EnumValue => this.value;
    }
}
