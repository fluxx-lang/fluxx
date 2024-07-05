using TypeTooling.CodeGeneration;
using TypeTooling.DotNet.RawTypes;
using TypeTooling.Helper;
using TypeTooling.RawTypes;
using TypeTooling.Types;

namespace TypeTooling.DotNet.Types
{
    public class DotNetEnumType : EnumTypeLazyLoaded
    {
        private readonly DotNetRawType _rawType;

        public DotNetEnumType(DotNetRawType rawType)
        {
            _rawType = rawType;
        }

        public override RawType UnderlyingType => _rawType;

        protected override EnumTypeData DoGetData()
        {
            var enumValues = new List<EnumValue>();
            foreach (string enumName in _rawType.GetEnumNames())
            {
                ExpressionAndHelpersCode expressionAndHelpersCode = new ExpressionAndHelpersCode(Code.EnumValueLiteral(_rawType, enumName));
                enumValues.Add(new EnumValue(enumName, expressionAndHelpersCode));
            }

            return new EnumTypeData(_rawType.FullName, enumValues);
        }
    }
}
