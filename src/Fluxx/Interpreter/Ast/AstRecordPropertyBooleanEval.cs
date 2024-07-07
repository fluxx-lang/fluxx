namespace Fluxx.Interpreter.Ast
{
    public class AstRecordPropertyBooleanEval : BooleanEval
    {
        private readonly string propertyName;
        private readonly ObjectEval astRecordObjectEval;

        public AstRecordPropertyBooleanEval(string propertyName, ObjectEval astRecordObjectEval)
        {
            this.propertyName = propertyName;
            this.astRecordObjectEval = astRecordObjectEval;
        }

        public override bool Eval()
        {
            var astRecord = (AstRecord)this.astRecordObjectEval.Eval();
            return (bool)astRecord.GetProperty(this.propertyName);
        }
    }
}
