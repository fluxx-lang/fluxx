using TypeTooling.Types;

namespace TypeTooling.CompanionType
{
    public interface ICustomLiteralParser
    {
        CustomLiteral Parse(string literal);
    }
}
