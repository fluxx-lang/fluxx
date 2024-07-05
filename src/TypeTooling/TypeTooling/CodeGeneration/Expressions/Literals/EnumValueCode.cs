namespace TypeTooling.CodeGeneration.Expressions.Literals
{
    public class EnumValueCode : ExpressionCode
    {
        private readonly object _enumTypeDescriptor;
        private readonly string _enumValue;

        public EnumValueCode(object enumTypeDescriptor, string enumValue)
        {
            this._enumTypeDescriptor = enumTypeDescriptor;
            this._enumValue = enumValue;
        }

        public object EnumTypeDescriptor => this._enumTypeDescriptor;

        public string EnumValue => this._enumValue;
    }
}
