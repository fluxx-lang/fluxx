

/**
 * @author Bret Johnson
 * @since 4/4/2015
 */
namespace Faml.Interpreter {
    public sealed class NotEqualsBooleanEval : BooleanEval {
        private readonly BooleanEval _leftOperand;
        private readonly BooleanEval _rightOperand;

        public NotEqualsBooleanEval(BooleanEval leftOperand, BooleanEval rightOperand) {
            this._leftOperand = leftOperand;
            this._rightOperand = rightOperand;
        }

        public override bool Eval() {
            return this._leftOperand.Eval() != this._rightOperand.Eval();
        }
    }

}
