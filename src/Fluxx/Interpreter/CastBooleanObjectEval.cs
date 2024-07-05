namespace Faml.Interpreter {
    public sealed class CastBooleanObjectEval : ObjectEval {
        private readonly BooleanEval _booleanEval;

        public CastBooleanObjectEval(BooleanEval booleanEval) {
            _booleanEval = booleanEval;
        }

        public override object Eval() {
            return _booleanEval.Eval();
        }
    }
}
