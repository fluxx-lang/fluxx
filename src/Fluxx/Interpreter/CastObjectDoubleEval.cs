namespace Faml.Interpreter
{
    public sealed class CastObjectDoubleEval : DoubleEval
    {
        private readonly ObjectEval objectEval;

        public CastObjectDoubleEval(ObjectEval objectEval)
        {
            this.objectEval = objectEval;
        }

        public override double Eval()
        {
            return (double)this.objectEval.Eval();
        }
    }
}
