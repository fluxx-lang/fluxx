namespace Faml.Interpreter {
    public sealed class CastIntObjectEval : ObjectEval {
        private readonly IntEval _intEval;

        public CastIntObjectEval(IntEval intEval) {
            _intEval = intEval;
        }

        public override object Eval() {
            return _intEval.Eval();
        }
    }
}
