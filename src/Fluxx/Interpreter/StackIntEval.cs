namespace Faml.Interpreter
{
    public class StackIntEval : IntEval
    {
        private readonly int stackOffset;

        internal StackIntEval(int stackOffset)
        {
            this.stackOffset = stackOffset;
        }

        public override int Eval()
        {
            return Context.IntStack[Context.BaseIndex + this.stackOffset];
        }
    }
}
