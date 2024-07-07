/**
 * @author Bret Johnson
 * @since 4/12/2015
 */

namespace Faml.Interpreter.Ast
{
    public class AstRecordPropertyBooleanEval : BooleanEval
    {
        private readonly string _propertyName;
        private readonly ObjectEval _astRecordObjectEval;

        public AstRecordPropertyBooleanEval(string propertyName, ObjectEval astRecordObjectEval)
        {
            this._propertyName = propertyName;
            this._astRecordObjectEval = astRecordObjectEval;
        }

        public override bool Eval()
        {
            var astRecord = (AstRecord) this._astRecordObjectEval.Eval();
            return (bool) astRecord.GetProperty(this._propertyName);
        }
    }
}
