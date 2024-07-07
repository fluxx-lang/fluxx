namespace Fluxx.Interpreter
{
    public class TrueEval : BooleanEval
    {
        public override bool Eval()
        {
            return true;
        }

        public override void Push()
        {
            Context.BooleanStack[Context.StackIndex++] = true;
        }
    }

}
