namespace Faml.Interpreter
{
    public sealed class CastByteObjectEval : ObjectEval
    {
        private readonly IntEval _intEval;

        public CastByteObjectEval(IntEval intEval)
        {
            this._intEval = intEval;
        }

        public override object Eval()
        {
            return (byte) this._intEval.Eval();
        }
    }
}
