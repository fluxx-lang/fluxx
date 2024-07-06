namespace TypeTooling.CodeGeneration.Expressions.Literals
{
    public class ByteLiteralCode(byte value) : LiteralCode
    {
        public byte Value => value;
    }
}
