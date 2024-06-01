/**
 * @author Bret Johnson
 * @since 7/27/2015
 */

namespace Faml.Interpreter.Ast {
    public class AstRecordPropertyIntSetter : AstRecordPropertySetter {
        private readonly IntEval _intEval;

        public AstRecordPropertyIntSetter(string propertyName, IntEval intEval) : base(propertyName) {
            _intEval = intEval;
        }

        public override void Invoke(AstRecord astRecord) {
            astRecord.SetProperty(PropertyName, _intEval.Eval());
        }
    }
}
