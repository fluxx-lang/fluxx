namespace Faml.Interpreter {
    public sealed class CastByteObjectEval : ObjectEval {
        private readonly IntEval _intEval;

        public CastByteObjectEval(IntEval intEval) {
            _intEval = intEval;
        }

        public override object Eval() {
            return (byte) _intEval.Eval();
        }
    }
}
