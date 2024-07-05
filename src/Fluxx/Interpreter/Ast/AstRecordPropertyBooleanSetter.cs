/**
 * @author Bret Johnson
 * @since 7/27/2015
 */

namespace Faml.Interpreter.Ast {
    public class AstRecordPropertyBooleanSetter : AstRecordPropertySetter {
        private readonly BooleanEval _booleanEval;

        public AstRecordPropertyBooleanSetter(string propertyName, BooleanEval booleanEval) : base(propertyName) {
            _booleanEval = booleanEval;
        }

        public override void Invoke(AstRecord astRecord) {
            astRecord.SetProperty(PropertyName, _booleanEval.Eval());
        }
    }
}
