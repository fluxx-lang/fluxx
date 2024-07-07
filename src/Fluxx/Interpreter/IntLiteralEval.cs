namespace Faml.Interpreter
{
    public class IntLiteralEval : IntEval
    {
        private readonly int value;

        public IntLiteralEval(int value)
        {
            this.value = value;
        }

        public override int Eval()
        {
            return this.value;
        }
    }
}
