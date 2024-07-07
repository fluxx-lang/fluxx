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
            this._conditionEvals = conditionEvals;
            this._valueEvals = valueEvals;
            this._elseEval = elseEval;
        }

        public override object Eval() {
            int length = this._conditionEvals.Length;

            for (int i = 0; i < length; i++) {
                if (this._conditionEvals[i].Eval())
                    return this._valueEvals[i].Eval();
            }

            return this._elseEval.Eval();
        }
    }
}
