namespace Fluxx.Interpreter
{
    public sealed class OrEval : BooleanEval
    {
        private readonly BooleanEval leftOperand;
        private readonly BooleanEval rightOperand;

        public OrEval(BooleanEval leftOperand, BooleanEval rightOperand)
        {
            this.leftOperand = leftOperand;
            this.rightOperand = rightOperand;
        }

        public override bool Eval()
        {
            return this.leftOperand.Eval() || this.rightOperand.Eval();
        }
    }
}
