namespace TypeTooling.CodeGeneration.Expressions.Literals
{
    public class StringLiteralCode : LiteralCode
    {
        private readonly string _value;

        public StringLiteralCode(string value)
        {
            _value = value;
        }

        public string Value => _value;
    }
}
