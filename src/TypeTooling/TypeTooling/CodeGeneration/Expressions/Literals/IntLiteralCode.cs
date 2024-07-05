namespace TypeTooling.CodeGeneration.Expressions.Literals
{
    public class IntLiteralCode : LiteralCode
    {
        private readonly int value;

        public IntLiteralCode(int value)
        {
            this.value = value;
        }

        public int Value => this.value;
    }
}
