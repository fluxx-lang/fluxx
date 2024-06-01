

/**
 * @author Bret Johnson
 * @since 4/4/2015
 */
namespace Faml.Interpreter {
    public class TrueEval : BooleanEval {
        public override bool Eval() {
            return true;
        }

        public override void Push() {
            Context.BooleanStack[Context.StackIndex++] = true;
        }
    }

}
