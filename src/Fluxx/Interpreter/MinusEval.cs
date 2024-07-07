namespace Fluxx.Interpreter
{
    public sealed class MinusEval : IntEval
    {
        private readonly IntEval leftOperand;
        private readonly IntEval rightOperand;

        public MinusEval(IntEval leftOperand, IntEval rightOperand)
        {
            this.leftOperand = leftOperand;
            this.rightOperand = rightOperand;
        }

        public override int Eval()
        {
            return this.leftOperand.Eval() - this.rightOperand.Eval();
        }
    }

}
