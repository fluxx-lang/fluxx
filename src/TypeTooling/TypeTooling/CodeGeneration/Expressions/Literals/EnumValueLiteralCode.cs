using TypeTooling.RawTypes;

namespace TypeTooling.CodeGeneration.Expressions.Literals
{
    public class EnumValueLiteralCode : LiteralCode {
        private readonly RawType _enumType;
        private readonly string _valueName;

        public EnumValueLiteralCode(RawType enumType, string valueName) {
            _enumType = enumType;
            _valueName = valueName;
        }

        public string ValueName => _valueName;
        
        public RawType EnumType => _enumType;
    }
}
