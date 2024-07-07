/**
 * @author Bret Johnson
 * @since 4/4/2015
 */
namespace Fluxx.Interpreter
{
    public sealed class IfObjectEval : ObjectEval
    {
        private readonly BooleanEval[] conditionEvals;
        private readonly ObjectEval[] valueEvals;
        private readonly ObjectEval elseEval;

        public IfObjectEval(BooleanEval[] conditionEvals, ObjectEval[] valueEvals, ObjectEval elseEval)
        {
            this.conditionEvals = conditionEvals;
            this.valueEvals = valueEvals;
            this.elseEval = elseEval;
        }

        public override object Eval()
        {
            int length = this.conditionEvals.Length;

            for (int i = 0; i < length; i++)
            {
                if (this.conditionEvals[i].Eval())
                {
                    return this.valueEvals[i].Eval();
                }
            }

            return this.elseEval.Eval();
        }
    }
}
