namespace Faml.Interpreter
{
    public sealed class OrEval : BooleanEval
    {
        private readonly BooleanEval _leftOperand;
        private readonly BooleanEval _rightOperand;

        public OrEval(BooleanEval leftOperand, BooleanEval rightOperand)
        {
            this._leftOperand = leftOperand;
            this._rightOperand = rightOperand;
        }

        public override bool Eval()
        {
            return this._leftOperand.Eval() || this._rightOperand.Eval();
        }
    }
}
