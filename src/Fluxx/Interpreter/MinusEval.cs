

/**
 * @author Bret Johnson
 * @since 4/4/2015
 */
namespace Faml.Interpreter {
    public sealed class MinusEval : IntEval {
        private readonly IntEval _leftOperand;
        private readonly IntEval _rightOperand;

        public MinusEval(IntEval leftOperand, IntEval rightOperand) {
            this._leftOperand = leftOperand;
            this._rightOperand = rightOperand;
        }

        public override int Eval() {
            return this._leftOperand.Eval() - this._rightOperand.Eval();
        }
    }

}
