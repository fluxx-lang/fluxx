namespace Fluxx.Interpreter
{
    public sealed class LessEval : BooleanEval
    {
        internal IntEval LeftOperand;
        internal IntEval RightOperand;

        public LessEval(IntEval leftOperand, IntEval rightOperand)
        {
            this.LeftOperand = leftOperand;
            this.RightOperand = rightOperand;
        }

        public override bool Eval()
        {
            return this.LeftOperand.Eval() < this.RightOperand.Eval();
        }
    }

}
