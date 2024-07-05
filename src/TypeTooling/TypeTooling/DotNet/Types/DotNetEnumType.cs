using TypeTooling.CodeGeneration;
using TypeTooling.DotNet.RawTypes;
using TypeTooling.Helper;
using TypeTooling.RawTypes;
using TypeTooling.Types;

namespace TypeTooling.DotNet.Types
{
    public class DotNetEnumType : EnumTypeLazyLoaded
    {
        private readonly DotNetRawType rawType;

        public DotNetEnumType(DotNetRawType rawType)
        {
            this.rawType = rawType;
        }

        public override RawType UnderlyingType => this.rawType;

        protected override EnumTypeData DoGetData()
        {
            var enumValues = new List<EnumValue>();
            foreach (string enumName in this.rawType.GetEnumNames())
            {
                ExpressionAndHelpersCode expressionAndHelpersCode = new ExpressionAndHelpersCode(Code.EnumValueLiteral(this.rawType, enumName));
                enumValues.Add(new EnumValue(enumName, expressionAndHelpersCode));
            }

            return new EnumTypeData(this.rawType.FullName, enumValues);
        }
    }
}
