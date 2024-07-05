namespace TypeTooling.CodeGeneration.Expressions.Literals
{
    public class BooleanLiteralCode : LiteralCode
    {
        private readonly bool value;

        public BooleanLiteralCode(bool value)
        {
            this.value = value;
        }

        public bool Value => this.value;
    }
}
