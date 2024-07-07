using System;

namespace Fluxx.Interpreter
{
    public abstract class ObjectEval : Eval
    {
        public abstract object Eval();

        public virtual void InitPrecreatedObject(object obj)
        {
            throw new Exception($"InitPrecreatedObject isn't supported for this ObjectEval type: {this.GetType().Name}");
        }

        public override void Push()
        {
            Context.ObjectStack[Context.StackIndex++] = this.Eval();
        }
    }
}
