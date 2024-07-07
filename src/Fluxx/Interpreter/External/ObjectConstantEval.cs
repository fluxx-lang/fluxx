namespace Faml.Interpreter.External
{
    public sealed class ObjectConstantEval : ObjectEval
    {
        private readonly object _obj;

        public ObjectConstantEval(object obj)
        {
            this._obj = obj;
        }

        public override object Eval()
        {
            return this._obj;
        }
    }
}
