using TypeTooling.RawTypes;

namespace TypeTooling.CodeGeneration.Expressions.Literals
{
    public class EnumValueLiteralCode(RawType enumType, string valueName) : LiteralCode
    {
        public string ValueName => valueName;

        public RawType EnumType => enumType;
    }
}
