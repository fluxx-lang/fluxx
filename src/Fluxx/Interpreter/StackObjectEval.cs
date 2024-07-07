namespace Faml.Interpreter
{
    public class StackObjectEval : ObjectEval
    {
        private readonly int stackOffset;

        internal StackObjectEval(int stackOffset)
        {
            this.stackOffset = stackOffset;
        }

        public override object Eval()
        {
            return Context.ObjectStack[Context.BaseIndex + this.stackOffset];
        }
    }
}
