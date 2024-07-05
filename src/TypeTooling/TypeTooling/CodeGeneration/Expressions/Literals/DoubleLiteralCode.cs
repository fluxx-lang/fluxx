namespace TypeTooling.CodeGeneration.Expressions.Literals
{
    public class DoubleLiteralCode : LiteralCode
    {
        private readonly double _value;

        public DoubleLiteralCode(double value)
        {
            this._value = value;
        }

        public double Value => this._value;
    }
}
