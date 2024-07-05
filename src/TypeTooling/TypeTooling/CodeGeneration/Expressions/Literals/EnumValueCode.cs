namespace TypeTooling.CodeGeneration.Expressions.Literals
{
    public class EnumValueCode : ExpressionCode
    {
        private readonly object enumTypeDescriptor;
        private readonly string enumValue;

        public EnumValueCode(object enumTypeDescriptor, string enumValue)
        {
            this.enumTypeDescriptor = enumTypeDescriptor;
            this.enumValue = enumValue;
        }

        public object EnumTypeDescriptor => this.enumTypeDescriptor;

        public string EnumValue => this.enumValue;
    }
}
