namespace Faml.Interpreter
{
    public sealed class NotEqualsIntEval : BooleanEval
    {
        private readonly IntEval leftOperand;
        private readonly IntEval rightOperand;

        public NotEqualsIntEval(IntEval leftOperand, IntEval rightOperand)
        {
            this.leftOperand = leftOperand;
            this.rightOperand = rightOperand;
        }

        public override bool Eval()
        {
            return this.leftOperand.Eval() != this.rightOperand.Eval();
        }
    }
}
