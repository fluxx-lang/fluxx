namespace Faml.Interpreter
{
    public sealed class GreaterEqualsEval : BooleanEval
    {
        internal IntEval LeftOperand;
        internal IntEval RightOperand;

        public GreaterEqualsEval(IntEval leftOperand, IntEval rightOperand)
        {
            this.LeftOperand = leftOperand;
            this.RightOperand = rightOperand;
        }

        public override bool Eval()
        {
            return this.LeftOperand.Eval() >= this.RightOperand.Eval();
        }
    }

}
