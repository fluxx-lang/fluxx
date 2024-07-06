namespace TypeTooling.CodeGeneration.Expressions.Literals
{
    public class EnumValueCode(object enumTypeDescriptor, string enumValue) : ExpressionCode
    {
        public object EnumTypeDescriptor => enumTypeDescriptor;

        public string EnumValue => enumValue;
    }
}
