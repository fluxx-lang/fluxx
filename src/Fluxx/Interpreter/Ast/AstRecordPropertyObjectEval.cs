namespace Faml.Interpreter.Ast
{
    public class AstRecordPropertyObjectEval : ObjectEval
    {
        private readonly string propertyName;
        private readonly ObjectEval astRecordObjectEval;

        public AstRecordPropertyObjectEval(string propertyName, ObjectEval astRecordObjectEval)
        {
            this.propertyName = propertyName;
            this.astRecordObjectEval = astRecordObjectEval;
        }

        public override object Eval()
        {
            var astRecord = (AstRecord)this.astRecordObjectEval.Eval();
            return astRecord.GetProperty(this.propertyName);
        }
    }
}
