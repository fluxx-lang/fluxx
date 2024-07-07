namespace Faml.Interpreter
{
    public abstract class BooleanEval : Eval
    {
        public abstract bool Eval();

        public override void Push()
        {
            Context.BooleanStack[Context.StackIndex++] = this.Eval();
        }
    }
}
