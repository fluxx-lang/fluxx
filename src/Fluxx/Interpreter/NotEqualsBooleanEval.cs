namespace Faml.Interpreter
{
    public sealed class NotEqualsBooleanEval : BooleanEval
    {
        private readonly BooleanEval leftOperand;
        private readonly BooleanEval rightOperand;

        public NotEqualsBooleanEval(BooleanEval leftOperand, BooleanEval rightOperand)
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
