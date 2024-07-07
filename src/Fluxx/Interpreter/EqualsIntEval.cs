

/**
 * @author Bret Johnson
 * @since 4/4/2015
 */
namespace Fluxx.Interpreter
{
    public sealed class EqualsIntEval : BooleanEval
    {
        internal IntEval LeftOperand;
        internal IntEval RightOperand;

        public EqualsIntEval(IntEval leftOperand, IntEval rightOperand)
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
