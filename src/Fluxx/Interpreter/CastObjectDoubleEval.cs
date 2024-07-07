namespace Faml.Interpreter {
    public sealed class CastObjectDoubleEval : DoubleEval {
        private readonly ObjectEval _objectEval;

        public CastObjectDoubleEval(ObjectEval objectEval) {
            this._objectEval = objectEval;
        }

        public override double Eval() {
            return (double) this._objectEval.Eval();
        }
    }
}
