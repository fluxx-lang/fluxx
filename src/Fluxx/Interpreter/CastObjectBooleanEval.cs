namespace Faml.Interpreter {
    public sealed class CastObjectBooleanEval : BooleanEval {
        private readonly ObjectEval _objectEval;

        public CastObjectBooleanEval(ObjectEval objectEval) {
            _objectEval = objectEval;
        }

        public override bool Eval() {
            return (bool) _objectEval.Eval();
        }
    }
}
