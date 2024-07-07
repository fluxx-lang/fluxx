namespace Faml.Interpreter
{
    public sealed class CastIntObjectEval : ObjectEval
    {
        private readonly IntEval intEval;

        public CastIntObjectEval(IntEval intEval)
        {
            this.intEval = intEval;
        }

        public override object Eval()
        {
            return this.intEval.Eval();
        }
    }
}
