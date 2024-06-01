namespace TypeTooling.CodeGeneration.Expressions.Literals
{
    public class DoubleLiteralCode : LiteralCode {
        private readonly double _value;

        public DoubleLiteralCode(double value) {
            _value = value;
        }

        public double Value => _value;
    }
}
