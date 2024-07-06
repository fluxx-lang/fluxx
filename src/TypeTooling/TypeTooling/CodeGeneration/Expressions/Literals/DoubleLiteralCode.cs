namespace TypeTooling.CodeGeneration.Expressions.Literals
{
    public class DoubleLiteralCode(double value) : LiteralCode
    {
        public double Value => value;
    }
}
