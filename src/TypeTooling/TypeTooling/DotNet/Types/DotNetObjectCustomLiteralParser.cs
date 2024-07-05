using TypeTooling.CompanionType;
using TypeTooling.Types;

namespace TypeTooling.DotNet.Types
{
    public class DotNetObjectCustomLiteralParser : CustomLiteralParser
    {
        private readonly ICustomLiteralParser companionType;

        public DotNetObjectCustomLiteralParser(ICustomLiteralParser companionType)
        {
            this.companionType = companionType;
        }

        public override CustomLiteral Parse(string literal)
        {
            return this.companionType.Parse(literal);
        }
    }
}
