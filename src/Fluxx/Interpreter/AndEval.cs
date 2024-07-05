/**
 * @author Bret Johnson
 * @since 4/4/2015
 */
namespace Faml.Interpreter {
    public sealed class AndEval : BooleanEval {
        internal BooleanEval LeftOperand;
        internal BooleanEval RightOperand;

        public AndEval(BooleanEval leftOperand, BooleanEval rightOperand) {
            this.LeftOperand = leftOperand;
            this.RightOperand = rightOperand;
        }

        public override bool Eval() {
            return LeftOperand.Eval() && RightOperand.Eval();
        }
    }
}
