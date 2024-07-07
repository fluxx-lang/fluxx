namespace Fluxx.Interpreter
{
    public sealed class CastObjectBooleanEval : BooleanEval
    {
        private readonly ObjectEval objectEval;

        public CastObjectBooleanEval(ObjectEval objectEval)
        {
            this.objectEval = objectEval;
        }

        public override bool Eval()
        {
            return (bool)this.objectEval.Eval();
        }
    }
}
