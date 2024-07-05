using TypeTooling.RawTypes;

namespace TypeTooling.CodeGeneration.Expressions.Literals
{
    public class EnumValueLiteralCode : LiteralCode
    {
        private readonly RawType enumType;
        private readonly string valueName;

        public EnumValueLiteralCode(RawType enumType, string valueName)
        {
            this.enumType = enumType;
            this.valueName = valueName;
        }

        public string ValueName => this.valueName;

        public RawType EnumType => this.enumType;
    }
}
