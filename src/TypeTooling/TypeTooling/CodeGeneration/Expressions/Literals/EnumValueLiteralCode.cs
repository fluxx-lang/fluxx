using TypeTooling.RawTypes;

namespace TypeTooling.CodeGeneration.Expressions.Literals
{
    public class EnumValueLiteralCode : LiteralCode
    {
        private readonly RawType _enumType;
        private readonly string _valueName;

        public EnumValueLiteralCode(RawType enumType, string valueName)
        {
            this._enumType = enumType;
            this._valueName = valueName;
        }

        public string ValueName => this._valueName;

        public RawType EnumType => this._enumType;
    }
}
