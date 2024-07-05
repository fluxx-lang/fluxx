namespace TypeTooling.CodeGeneration.Expressions.Literals
{
    public class ByteLiteralCode : LiteralCode
    {
        private readonly byte value;

        public ByteLiteralCode(byte value)
        {
            this.value = value;
        }

        public byte Value => this.value;
    }
}
