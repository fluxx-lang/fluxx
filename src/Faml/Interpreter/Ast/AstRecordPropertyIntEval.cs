/**
 * @author Bret Johnson
 * @since 4/12/2015
 */

namespace Faml.Interpreter.Ast {
    public class AstRecordPropertyIntEval : IntEval {
        private readonly string _propertyName;
        private readonly ObjectEval _astRecordObjectEval;

        public AstRecordPropertyIntEval(string propertyName, ObjectEval astRecordObjectEval) {
            _propertyName = propertyName;
            _astRecordObjectEval = astRecordObjectEval;
        }

        public override int Eval() {
            object obj = _astRecordObjectEval.Eval();
            var astRecord = (AstRecord) obj;
            return (int) astRecord.GetProperty(_propertyName);
        }
    }
}
