namespace TypeTooling.CodeGeneration.Expressions.Literals
{
    public class StringLiteralCode : LiteralCode
    {
        private readonly string value;

        public StringLiteralCode(string value)
        {
            this.value = value;
        }

        public string Value => this.value;
    }
}
