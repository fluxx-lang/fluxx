namespace Fluxx.Interpreter
{
    public sealed class CastObjectIntEval : IntEval
    {
        private readonly ObjectEval objectEval;

        public CastObjectIntEval(ObjectEval objectEval)
        {
            this.objectEval = objectEval;
        }

        public override int Eval()
        {
            return (int)this.objectEval.Eval();
        }
    }
}
