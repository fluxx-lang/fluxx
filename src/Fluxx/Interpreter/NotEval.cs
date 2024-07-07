namespace Fluxx.Interpreter
{
    public sealed class NotEval : BooleanEval
    {
        private readonly BooleanEval operand;

        public NotEval(BooleanEval operand)
        {
            this.operand = operand;
        }

        public override bool Eval()
        {
            return !this.operand.Eval();
        }
    }

}
