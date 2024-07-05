

/**
 * @author Bret Johnson
 * @since 4/12/2015
 */
namespace Faml.Interpreter {
    public class StackIntEval : IntEval {
        private readonly int _stackOffset;

        internal StackIntEval(int stackOffset) {
            this._stackOffset = stackOffset;
        }

        public override int Eval() {
            return Context.IntStack[Context.BaseIndex + _stackOffset];
        }
    }
}
