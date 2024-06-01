namespace TypeTooling.CodeGeneration.Expressions.Literals
{
    public class EnumValueCode : ExpressionCode {
        private readonly object _enumTypeDescriptor;
        private readonly string _enumValue;

        public EnumValueCode(object enumTypeDescriptor, string enumValue) {
            _enumTypeDescriptor = enumTypeDescriptor;
            _enumValue = enumValue;
        }

        public object EnumTypeDescriptor => _enumTypeDescriptor;

        public string EnumValue => _enumValue;
    }
}
