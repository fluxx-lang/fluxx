namespace Faml.Interpreter {
    public sealed class CastObjectIntEval : IntEval {
        private readonly ObjectEval _objectEval;

        public CastObjectIntEval(ObjectEval objectEval) {
            this._objectEval = objectEval;
        }

        public override int Eval() {
            return (int) this._objectEval.Eval();
        }
    }
}
