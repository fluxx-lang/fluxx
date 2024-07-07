namespace Faml.Interpreter.Ast
{
    public class AstRecordPropertyDoubleEval : DoubleEval
    {
        private readonly string _propertyName;
        private readonly ObjectEval _astRecordObjectEval;

        public AstRecordPropertyDoubleEval(string propertyName, ObjectEval astRecordObjectEval)
        {
            this._propertyName = propertyName;
            this._astRecordObjectEval = astRecordObjectEval;
        }

        public override double Eval()
        {
            var astRecord = (AstRecord)this._astRecordObjectEval.Eval();
            return (double)astRecord.GetProperty(this._propertyName);
        }
    }
}
