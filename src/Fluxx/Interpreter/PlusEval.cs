/**
 * @author Bret Johnson
 * @since 4/4/2015
 */
namespace Faml.Interpreter {
    public sealed class PlusEval : IntEval {
        internal readonly IntEval LeftOperand;
        internal readonly IntEval RightOperand;

        public PlusEval(IntEval leftOperand, IntEval rightOperand) {
            this.LeftOperand = leftOperand;
            this.RightOperand = rightOperand;
        }

        public override int Eval() {
            return LeftOperand.Eval() + RightOperand.Eval();
        }
    }
}
