

/**
 * @author Bret Johnson
 * @since 4/4/2015
 */
namespace Faml.Interpreter {
    public sealed class TimesEval : IntEval {
        internal IntEval LeftOperand;
        internal IntEval RightOperand;

        public TimesEval(IntEval leftOperand, IntEval rightOperand) {
            this.LeftOperand = leftOperand;
            this.RightOperand = rightOperand;
        }

        public override int Eval() {
            return LeftOperand.Eval() * RightOperand.Eval();
        }
    }

}
