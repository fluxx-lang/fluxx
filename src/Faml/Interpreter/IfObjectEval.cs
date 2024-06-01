/**
 * @author Bret Johnson
 * @since 4/4/2015
 */
namespace Faml.Interpreter {
    public sealed class IfObjectEval : ObjectEval {
        private readonly BooleanEval[] _conditionEvals;
        private readonly ObjectEval[] _valueEvals;
        private readonly ObjectEval _elseEval;

        public IfObjectEval(BooleanEval[] conditionEvals, ObjectEval[] valueEvals, ObjectEval elseEval) {
            _conditionEvals = conditionEvals;
            _valueEvals = valueEvals;
            _elseEval = elseEval;
        }

        public override object Eval() {
            int length = _conditionEvals.Length;

            for (int i = 0; i < length; i++) {
                if (_conditionEvals[i].Eval())
                    return _valueEvals[i].Eval();
            }

            return _elseEval.Eval();
        }
    }
}
