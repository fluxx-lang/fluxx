

/**
 * Return the value of a boolean, which is on the stack.
 *
 * @author Bret Johnson
 * @since 4/4/2015
 */
namespace Fluxx.Interpreter
{
    public class StackBooleanEval : BooleanEval
    {
        private readonly int stackOffset;

        internal StackBooleanEval(int stackOffset)
        {
            this.stackOffset = stackOffset;
        }

        public override bool Eval()
        {
            return Context.BooleanStack[Context.BaseIndex + this.stackOffset];
        }
    }
}
