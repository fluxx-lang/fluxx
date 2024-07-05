/**
 * @author Bret Johnson
 * @since 4/12/2015
 */

namespace Faml.Interpreter.Ast {
    public class AstRecordPropertyDoubleEval : DoubleEval {
        private readonly string _propertyName;
        private readonly ObjectEval _astRecordObjectEval;

        public AstRecordPropertyDoubleEval(string propertyName, ObjectEval astRecordObjectEval) {
            _propertyName = propertyName;
            _astRecordObjectEval = astRecordObjectEval;
        }

        public override double Eval() {
            var astRecord = (AstRecord) _astRecordObjectEval.Eval();
            return (double) astRecord.GetProperty(_propertyName);
        }
    }
}
