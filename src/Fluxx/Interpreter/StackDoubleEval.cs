namespace Fluxx.Interpreter
{
    public class StackDoubleEval : DoubleEval
    {
        private readonly int stackOffset;

        internal StackDoubleEval(int stackOffset)
        {
            this.stackOffset = stackOffset;
        }

        public override double Eval()
        {
            return Context.IntStack[Context.BaseIndex + this.stackOffset];
        }
    }
}
