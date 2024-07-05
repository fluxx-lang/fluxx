namespace TypeTooling.CodeGeneration.Expressions.Literals
{
    public class IntLiteralCode : LiteralCode
    {
        private readonly int _value;

        public IntLiteralCode(int value)
        {
            this._value = value;
        }

        public int Value => this._value;
    }
}
