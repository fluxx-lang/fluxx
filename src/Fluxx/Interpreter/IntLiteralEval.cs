namespace Faml.Interpreter
{
    public class IntLiteralEval : IntEval
    {
        private readonly int _value;

        public IntLiteralEval(int value)
        {
            this._value = value;
        }

        public override int Eval()
        {
            return this._value;
        }
    }
}
