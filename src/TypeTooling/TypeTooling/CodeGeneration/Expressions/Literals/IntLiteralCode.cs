namespace TypeTooling.CodeGeneration.Expressions.Literals
{
    public class IntLiteralCode(int value) : LiteralCode
    {
        public int Value => value;
    }
}
