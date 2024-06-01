/**
 * @author Bret Johnson
 * @since 4/12/2015
 */

namespace Faml.Interpreter.Ast {
    public class AstRecordPropertyBooleanEval : BooleanEval {
        private readonly string _propertyName;
        private readonly ObjectEval _astRecordObjectEval;

        public AstRecordPropertyBooleanEval(string propertyName, ObjectEval astRecordObjectEval) {
            _propertyName = propertyName;
            _astRecordObjectEval = astRecordObjectEval;
        }

        public override bool Eval() {
            var astRecord = (AstRecord) _astRecordObjectEval.Eval();
            return (bool) astRecord.GetProperty(_propertyName);
        }
    }
}
