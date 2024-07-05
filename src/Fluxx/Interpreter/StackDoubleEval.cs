

/**
 * @author Bret Johnson
 * @since 4/12/2015
 */
namespace Faml.Interpreter {
    public class StackDoubleEval : DoubleEval {
        private readonly int _stackOffset;

        internal StackDoubleEval(int stackOffset) {
            this._stackOffset = stackOffset;
        }

        public override double Eval() {
            return Context.IntStack[Context.BaseIndex + _stackOffset];
        }
    }
}
