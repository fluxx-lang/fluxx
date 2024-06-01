namespace TypeTooling.CodeGeneration.Expressions.Literals
{
    public class IntLiteralCode : LiteralCode {
        private readonly int _value;

        public IntLiteralCode(int value) {
            _value = value;
        }

        public int Value => _value;
    }
}
