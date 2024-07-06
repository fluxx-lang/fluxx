namespace TypeTooling.CodeGeneration.Expressions.Literals
{
    public class StringLiteralCode(string value) : LiteralCode
    {
        public string Value => value;
    }
}
