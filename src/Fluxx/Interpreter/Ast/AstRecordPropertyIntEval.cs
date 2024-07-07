namespace Faml.Interpreter.Ast
{
    public class AstRecordPropertyIntEval : IntEval
    {
        private readonly string _propertyName;
        private readonly ObjectEval _astRecordObjectEval;

        public AstRecordPropertyIntEval(string propertyName, ObjectEval astRecordObjectEval)
        {
            this._propertyName = propertyName;
            this._astRecordObjectEval = astRecordObjectEval;
        }

        public override int Eval()
        {
            object obj = this._astRecordObjectEval.Eval();
            var astRecord = (AstRecord)obj;
            return (int)astRecord.GetProperty(this._propertyName);
        }
    }
}
