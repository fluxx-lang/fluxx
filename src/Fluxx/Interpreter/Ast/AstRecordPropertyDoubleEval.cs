namespace Fluxx.Interpreter.Ast
{
    public class AstRecordPropertyDoubleEval : DoubleEval
    {
        private readonly string propertyName;
        private readonly ObjectEval astRecordObjectEval;

        public AstRecordPropertyDoubleEval(string propertyName, ObjectEval astRecordObjectEval)
        {
            this.propertyName = propertyName;
            this.astRecordObjectEval = astRecordObjectEval;
        }

        public override double Eval()
        {
            var astRecord = (AstRecord)this.astRecordObjectEval.Eval();
            return (double)astRecord.GetProperty(this.propertyName);
        }
    }
}
