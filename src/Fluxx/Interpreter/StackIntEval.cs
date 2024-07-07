namespace Faml.Interpreter
{
    public class StackIntEval : IntEval
    {
        private readonly int _stackOffset;

        internal StackIntEval(int stackOffset)
        {
            this._stackOffset = stackOffset;
        }

        public override int Eval()
        {
            return Context.IntStack[Context.BaseIndex + this._stackOffset];
        }
    }
}
