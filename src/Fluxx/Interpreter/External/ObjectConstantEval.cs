namespace Faml.Interpreter.External
{
    public sealed class ObjectConstantEval : ObjectEval
    {
        private readonly object obj;

        public ObjectConstantEval(object obj)
        {
            this.obj = obj;
        }

        public override object Eval()
        {
            return this.obj;
        }
    }
}
