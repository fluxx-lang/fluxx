/**
 * @author Bret Johnson
 * @since 4/4/2015
 */
using System.Text;
namespace Faml.Interpreter {
    public sealed class InterpolatedStringEval : ObjectEval {
        internal string[] StringFragments;
        internal ObjectEval[] Expressions;

        public InterpolatedStringEval(string[] stringFragments, ObjectEval[] expressions) {
            this.StringFragments = stringFragments;
            this.Expressions = expressions;
        }

        public override object Eval() {
            // TODO: Potentially create buffer with initial size as an optimization
            StringBuilder buffer = new StringBuilder();

            int length = Expressions.Length;
            for (int i = 0; i < length; i++) {
                buffer.Append(StringFragments[i]);
                buffer.Append(Expressions[i].Eval());
            }
            buffer.Append(StringFragments[length]);

            return buffer.ToString();
        }
    }
}
