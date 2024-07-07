

/**
 * @author Bret Johnson
 * @since 4/4/2015
 */
namespace Faml.Interpreter {
    public sealed class NotEval : BooleanEval {
        private readonly BooleanEval _operand;

        public NotEval(BooleanEval operand) {
            this._operand = operand;
        }

        public override bool Eval() {
            return !this._operand.Eval();
        }
    }

}
