/**
 * @author Bret Johnson
 * @since 4/4/2015
 */
namespace Faml.Interpreter
{
    public abstract class DoubleEval : Eval
    {
        public abstract double Eval();

        public override void Push()
        {
            Context.DoubleStack[Context.StackIndex++] = this.Eval();
        }
    }
}
