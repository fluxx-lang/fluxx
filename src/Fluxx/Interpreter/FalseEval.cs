/**
 * @author Bret Johnson
 * @since 4/4/2015
 */

namespace Faml.Interpreter
{
    public class FalseEval : BooleanEval
    {
        public override bool Eval()
        {
            return false;
        }

        public override void Push()
        {
            Context.BooleanStack[Context.StackIndex++] = false;
        }
    }
}
