namespace TypeTooling.CodeGeneration.Expressions.Literals
{
    public class DoubleLiteralCode : LiteralCode
    {
        private readonly double value;

        public DoubleLiteralCode(double value)
        {
            this.value = value;
        }

        public double Value => this.value;
    }
}
