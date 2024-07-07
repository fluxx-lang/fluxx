

/**
 * @author Bret Johnson
 * @since 4/4/2015
 */
namespace Faml.Interpreter {
    public sealed class GreaterEval : BooleanEval {
        internal IntEval LeftOperand;
        internal IntEval RightOperand;

        public GreaterEval(IntEval leftOperand, IntEval rightOperand) {
            this.LeftOperand = leftOperand;
            this.RightOperand = rightOperand;
        }

        public override bool Eval() {
            return this.LeftOperand.Eval() > this.RightOperand.Eval();
        }
    }

}
