namespace TypeTooling.CodeGeneration.Expressions.Literals
{
    public class BooleanLiteralCode : LiteralCode {
        private readonly bool _value;

        public BooleanLiteralCode(bool value) {
            _value = value;
        }

        public bool Value => _value;
    }
}
