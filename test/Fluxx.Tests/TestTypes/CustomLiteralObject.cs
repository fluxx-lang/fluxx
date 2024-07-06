using TypeTooling.CodeGeneration.Expressions;
using TypeTooling.CompanionType;
using TypeTooling.DotNet.CodeGeneration;
using TypeTooling.DotNet.RawTypes.Reflection;
using TypeTooling.Types;

namespace Faml.Tests.TestTypes {
    class CustomLiteralObject {
    }

    class CustomLiteralObjectTypeTooling : ICustomLiteralParser {
        public CustomLiteral Parse(string literal) {
            if (literal == "!!!") {
                var myType = new ReflectionDotNetRawType(typeof(CustomLiteralObject));
                ExpressionCode newCode = DotNetCode.New(myType, new string[] { });
                return new CustomLiteral(null, newCode);
            }
            else return CustomLiteral.SingleError("Invalid value");
        }
    }
}
