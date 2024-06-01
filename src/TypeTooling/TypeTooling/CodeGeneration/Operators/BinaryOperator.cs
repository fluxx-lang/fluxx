using System.Collections.Generic;

/**
 * @author Bret Johnson
 * @since 6/28/2014 11:10 PM
 */

namespace TypeTooling.CodeGeneration.Operators {
    public class BinaryOperator : Operator {
        // Boolean operators

        public static BinaryOperator And = new BinaryOperator("&&");
        public static BinaryOperator Or = new BinaryOperator("||");

        public static BinaryOperator Equals = new BinaryOperator("==");
        public static BinaryOperator NotEquals = new BinaryOperator("!=");

        public static BinaryOperator LessThan = new BinaryOperator("<");
        public static BinaryOperator LessThanOrEqual = new BinaryOperator("<=");
        public static BinaryOperator GreaterThan = new BinaryOperator(">");
        public static BinaryOperator GreaterThanOrEqual = new BinaryOperator(">=");

        // Numerical operators

        public static BinaryOperator Multiply = new BinaryOperator("*");
        public static BinaryOperator Divide = new BinaryOperator("/");
        public static BinaryOperator Remainder = new BinaryOperator("%");

        public static BinaryOperator Add = new BinaryOperator("+");
        public static BinaryOperator Subtract = new BinaryOperator("-");

        public BinaryOperator(string defaultStringRepresentation) : base(defaultStringRepresentation)
        {
        }
    }
}
