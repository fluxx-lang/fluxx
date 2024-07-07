namespace Fluxx.Interpreter
{
    public sealed class EqualsBooleanEval : BooleanEval
    {
        internal BooleanEval  LeftOperand;
        internal BooleanEval RightOperand;

        public EqualsBooleanEval(BooleanEval leftOperand, BooleanEval rightOperand)
        {
            this.LeftOperand = leftOperand;
            this.RightOperand = rightOperand;
        }

        public override bool Eval()
        {
            return this.LeftOperand.Eval() == this.RightOperand.Eval();
        }
    }

}
