namespace Faml.Interpreter
{
    public sealed class PlusEval : IntEval
    {
        internal readonly IntEval LeftOperand;
        internal readonly IntEval RightOperand;

        public PlusEval(IntEval leftOperand, IntEval rightOperand)
        {
            this.LeftOperand = leftOperand;
            this.RightOperand = rightOperand;
        }

        public override int Eval()
        {
            return this.LeftOperand.Eval() + this.RightOperand.Eval();
        }
    }
}
