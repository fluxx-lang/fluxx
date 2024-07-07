

/**
 * @author Bret Johnson
 * @since 4/4/2015
 */
namespace Faml.Interpreter
{
    public class StringLiteralEval : ObjectEval
    {
        private readonly string _value;

        public StringLiteralEval(string value)
        {
            this._value = value;
        }

        public override object Eval()
        {
            return this._value;
        }
    }

}
