

/**
 * Return the value of a boolean, which is on the stack.
 *
 * @author Bret Johnson
 * @since 4/4/2015
 */
namespace Faml.Interpreter {
    public class StackBooleanEval : BooleanEval {
        private readonly int _stackOffset;

        internal StackBooleanEval(int stackOffset) {
            this._stackOffset = stackOffset;
        }

        public override bool Eval() {
            return Context.BooleanStack[Context.BaseIndex + _stackOffset];
        }
    }
}
