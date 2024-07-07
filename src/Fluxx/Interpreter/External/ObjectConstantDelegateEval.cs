using System;

namespace Fluxx.Interpreter.External
{
    public sealed class ObjectConstantDelegateEval : ObjectEval
    {
        private readonly Delegate dlgate;

        public ObjectConstantDelegateEval(Delegate dlgate)
        {
            this.dlgate = dlgate;
        }

        public override object Eval()
        {
            return this.dlgate.DynamicInvoke(null);
        }
    }
}
