

/**
 * @author Bret Johnson
 * @since 4/4/2015
 */
namespace Faml.Interpreter {
    public sealed class LessEval : BooleanEval {
        internal IntEval LeftOperand;
        internal IntEval RightOperand;

        public LessEval(IntEval leftOperand, IntEval rightOperand) {
            this.LeftOperand = leftOperand;
            this.RightOperand = rightOperand;
        }

        public override bool Eval() {
            return LeftOperand.Eval() < RightOperand.Eval();
        }
    }

}
