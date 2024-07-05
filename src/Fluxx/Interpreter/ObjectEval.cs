/**
 * @author Bret Johnson
 * @since 4/4/2015
 */

using System;

namespace Faml.Interpreter {
    public abstract class ObjectEval : Eval {
        public abstract object Eval();

        public virtual void InitPrecreatedObject(object obj) {
            throw new Exception($"InitPrecreatedObject isn't supported for this ObjectEval type: {this.GetType().Name}");
        }

        public override void Push() {
            Context.ObjectStack[Context.StackIndex++] = Eval();
        }
    }
}
