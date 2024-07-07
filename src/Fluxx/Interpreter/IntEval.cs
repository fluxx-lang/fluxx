/**
 * @author Bret Johnson
 * @since 4/4/2015
 */
namespace Fluxx.Interpreter
{
    public abstract class IntEval : Eval
    {
        public abstract int Eval();

        public override void Push()
        {
            Context.IntStack[Context.StackIndex++] = this.Eval();
        }
    }
}
