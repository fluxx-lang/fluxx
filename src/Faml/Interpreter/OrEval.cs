/**
 * @author Bret Johnson
 * @since 4/4/2015
 */

namespace Faml.Interpreter {
    public sealed class OrEval : BooleanEval {
        private readonly BooleanEval _leftOperand;
        private readonly BooleanEval _rightOperand;

        public OrEval(BooleanEval leftOperand, BooleanEval rightOperand) {
            _leftOperand = leftOperand;
            _rightOperand = rightOperand;
        }

        public override bool Eval() {
            return _leftOperand.Eval() || _rightOperand.Eval();
        }
    }
}