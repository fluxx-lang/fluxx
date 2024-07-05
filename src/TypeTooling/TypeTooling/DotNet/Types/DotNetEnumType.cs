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
            this._rawType = rawType;
        }

        public override RawType UnderlyingType => this._rawType;

        protected override EnumTypeData DoGetData()
        {
            var enumValues = new List<EnumValue>();
            foreach (string enumName in this._rawType.GetEnumNames())
            {
                ExpressionAndHelpersCode expressionAndHelpersCode = new ExpressionAndHelpersCode(Code.EnumValueLiteral(this._rawType, enumName));
                enumValues.Add(new EnumValue(enumName, expressionAndHelpersCode));
            }

            return new EnumTypeData(this._rawType.FullName, enumValues);
        }
    }
}
