namespace Fluxx.Interpreter
{
    public sealed class CastBooleanObjectEval : ObjectEval
    {
        private readonly BooleanEval booleanEval;

        public CastBooleanObjectEval(BooleanEval booleanEval)
        {
            this.booleanEval = booleanEval;
        }

        public override object Eval()
        {
            return this.booleanEval.Eval();
        }
    }
}
