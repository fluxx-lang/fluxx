namespace TypeTooling.CodeGeneration.Operators
{
    public class UnaryOperator(string defaultStringRepresentation) : Operator(defaultStringRepresentation)
    {
        public static UnaryOperator UnaryMinus = new UnaryOperator("-");
        public static UnaryOperator Not = new UnaryOperator("!");
    }
}
