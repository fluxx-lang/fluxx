namespace Fluxx.Interpreter
{
    public sealed class TimesEval : IntEval
    {
        internal IntEval LeftOperand;
        internal IntEval RightOperand;

        public TimesEval(IntEval leftOperand, IntEval rightOperand)
        {
            this.LeftOperand = leftOperand;
            this.RightOperand = rightOperand;
        }

        public override int Eval()
        {
            return this.LeftOperand.Eval() * this.RightOperand.Eval();
        }
    }

}
