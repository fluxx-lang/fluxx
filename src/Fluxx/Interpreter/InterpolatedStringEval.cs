using System.Text;
namespace Faml.Interpreter
{
    public sealed class InterpolatedStringEval : ObjectEval
    {
        internal string[] StringFragments;
        internal ObjectEval[] Expressions;

        public InterpolatedStringEval(string[] stringFragments, ObjectEval[] expressions)
        {
            this.StringFragments = stringFragments;
            this.Expressions = expressions;
        }

        public override object Eval()
        {
            // TODO: Potentially create buffer with initial size as an optimization
            StringBuilder buffer = new StringBuilder();

            int length = this.Expressions.Length;
            for (int i = 0; i < length; i++)
            {
                buffer.Append(this.StringFragments[i]);
                buffer.Append(this.Expressions[i].Eval());
            }

            buffer.Append(this.StringFragments[length]);

            return buffer.ToString();
        }
    }
}
