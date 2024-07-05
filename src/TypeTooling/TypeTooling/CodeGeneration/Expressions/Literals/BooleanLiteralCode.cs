namespace TypeTooling.CodeGeneration.Expressions.Literals
{
    public class BooleanLiteralCode : LiteralCode
    {
        private readonly bool _value;

        public BooleanLiteralCode(bool value)
        {
            this._value = value;
        }

        public bool Value => this._value;
    }
}
