/**
 * @author Bret Johnson
 * @since 4/4/2015
 */

using System;

namespace Faml.Interpreter.External {
    public sealed class ObjectConstantDelegateEval : ObjectEval {
        private readonly Delegate _dlgate;

        public ObjectConstantDelegateEval(Delegate dlgate) {
            _dlgate = dlgate;
        }

        public override object Eval() {
            return _dlgate.DynamicInvoke(null);
        }
    }
}
