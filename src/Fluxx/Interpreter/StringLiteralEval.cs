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
