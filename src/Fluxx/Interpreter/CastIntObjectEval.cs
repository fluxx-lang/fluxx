namespace Faml.Interpreter {
    public sealed class CastIntObjectEval : ObjectEval {
        private readonly IntEval _intEval;

        public CastIntObjectEval(IntEval intEval) {
            this._intEval = intEval;
        }

        public override object Eval() {
            return this._intEval.Eval();
        }
    }
}
