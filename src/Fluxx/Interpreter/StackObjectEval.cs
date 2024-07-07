namespace Faml.Interpreter
{
    public class StackObjectEval : ObjectEval
    {
        private readonly int _stackOffset;

        internal StackObjectEval(int stackOffset)
        {
            this._stackOffset = stackOffset;
        }

        public override object Eval()
        {
            return Context.ObjectStack[Context.BaseIndex + this._stackOffset];
        }
    }
}
