namespace TypeTooling.CodeGeneration.Operators {
    public class UnaryOperator : Operator {
        public static UnaryOperator UnaryMinus = new UnaryOperator("-");
        public static UnaryOperator Not = new UnaryOperator("!");

        public UnaryOperator(string defaultStringRepresentation) : base(defaultStringRepresentation)
        {
        }
    }
}
