namespace Faml.Interpreter
{
    public sealed class CastByteObjectEval : ObjectEval
    {
        private readonly IntEval intEval;

        public CastByteObjectEval(IntEval intEval)
        {
            this.intEval = intEval;
        }

        public override object Eval()
        {
            return (byte)this.intEval.Eval();
        }
    }
}
