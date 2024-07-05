namespace Faml.Interpreter {
    public sealed class CastObjectIntEval : IntEval {
        private readonly ObjectEval _objectEval;

        public CastObjectIntEval(ObjectEval objectEval) {
            _objectEval = objectEval;
        }

        public override int Eval() {
            return (int) _objectEval.Eval();
        }
    }
}
