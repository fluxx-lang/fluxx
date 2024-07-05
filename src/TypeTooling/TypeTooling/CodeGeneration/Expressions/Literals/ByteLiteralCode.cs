namespace TypeTooling.CodeGeneration.Expressions.Literals
{
    public class ByteLiteralCode : LiteralCode
    {
        private readonly byte _value;

        public ByteLiteralCode(byte value)
        {
            this._value = value;
        }

        public byte Value => this._value;
    }
}
