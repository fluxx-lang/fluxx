

/**
 * @author Bret Johnson
 * @since 4/4/2015
 */
namespace Faml.Interpreter {
    public sealed class MinusEval : IntEval {
        private readonly IntEval _leftOperand;
        private readonly IntEval _rightOperand;

        public MinusEval(IntEval leftOperand, IntEval rightOperand) {
            _leftOperand = leftOperand;
            _rightOperand = rightOperand;
        }

        public override int Eval() {
            return _leftOperand.Eval() - _rightOperand.Eval();
        }
    }

}
