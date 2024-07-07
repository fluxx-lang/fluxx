namespace Fluxx.Interpreter.Ast
{
    public class AstRecordPropertyIntEval : IntEval
    {
        private readonly string propertyName;
        private readonly ObjectEval astRecordObjectEval;

        public AstRecordPropertyIntEval(string propertyName, ObjectEval astRecordObjectEval)
        {
            this.propertyName = propertyName;
            this.astRecordObjectEval = astRecordObjectEval;
        }

        public override int Eval()
        {
            object obj = this.astRecordObjectEval.Eval();
            var astRecord = (AstRecord)obj;
            return (int)astRecord.GetProperty(this.propertyName);
        }
    }
}
