namespace TypeTooling.CodeGeneration.Expressions.Literals
{
    public class ByteLiteralCode : LiteralCode {
        private readonly byte _value;

        public ByteLiteralCode(byte value) {
            _value = value;
        }

        public byte Value => _value;
    }
}
