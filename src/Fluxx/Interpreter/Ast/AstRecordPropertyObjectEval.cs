namespace Faml.Interpreter.Ast
{
    public class AstRecordPropertyObjectEval : ObjectEval
    {
        private readonly string _propertyName;
        private readonly ObjectEval _astRecordObjectEval;

        public AstRecordPropertyObjectEval(string propertyName, ObjectEval astRecordObjectEval)
        {
            this._propertyName = propertyName;
            this._astRecordObjectEval = astRecordObjectEval;
        }

        public override object Eval()
        {
            var astRecord = (AstRecord)this._astRecordObjectEval.Eval();
            return astRecord.GetProperty(this._propertyName);
        }
    }
}
