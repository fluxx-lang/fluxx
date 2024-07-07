/**
 * @author Bret Johnson
 * @since 4/4/2015
 */

namespace Faml.Interpreter.External {
    public sealed class ObjectConstantEval : ObjectEval {
        private readonly object _obj;

        public ObjectConstantEval(object obj) {
            this._obj = obj;
        }

        public override object Eval() {
            return this._obj;
        }
    }
}
