namespace Fluxx.Interpreter
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
