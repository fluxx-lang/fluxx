namespace Faml.Interpreter
{
    public class StringLiteralEval : ObjectEval
    {
        private readonly string value;

        public StringLiteralEval(string value)
        {
            this.value = value;
        }

        public override object Eval()
        {
            return this.value;
        }
    }

}
