using System;

namespace Faml.Interpreter.External
{
    public sealed class ObjectConstantDelegateEval : ObjectEval
    {
        private readonly Delegate _dlgate;

        public ObjectConstantDelegateEval(Delegate dlgate)
        {
            this._dlgate = dlgate;
        }

        public override object Eval()
        {
            return this._dlgate.DynamicInvoke(null);
        }
    }
}
