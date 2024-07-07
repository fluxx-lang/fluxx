/**
 * @author Bret Johnson
 * @since 7/27/2015
 */

namespace Faml.Interpreter.Ast {
    public class AstRecordPropertyObjectSetter : AstRecordPropertySetter {
        private readonly ObjectEval _objectEval;

        public AstRecordPropertyObjectSetter(string propertyName, ObjectEval objectEval) : base(propertyName) {
            this._objectEval = objectEval;
        }

        public override void Invoke(AstRecord astRecord) {
            astRecord.SetProperty(this.PropertyName, this._objectEval.Eval());
        }
    }
}
