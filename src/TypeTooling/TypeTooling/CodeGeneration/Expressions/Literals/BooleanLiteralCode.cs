namespace TypeTooling.CodeGeneration.Expressions.Literals
{
    public class BooleanLiteralCode(bool value) : LiteralCode
    {
        public bool Value => value;
    }
}
