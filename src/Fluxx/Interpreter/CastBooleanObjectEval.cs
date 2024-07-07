namespace Faml.Interpreter {
    public sealed class CastBooleanObjectEval : ObjectEval {
        private readonly BooleanEval _booleanEval;

        public CastBooleanObjectEval(BooleanEval booleanEval) {
            this._booleanEval = booleanEval;
        }

        public override object Eval() {
            return this._booleanEval.Eval();
        }
    }
}
