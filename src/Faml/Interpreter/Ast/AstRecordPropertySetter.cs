/**
 * @author Bret Johnson
 * @since 7/27/2015
 */

namespace Faml.Interpreter.Ast {
    public abstract class AstRecordPropertySetter {
        protected internal readonly string PropertyName;

        protected AstRecordPropertySetter(string propertyName) {
            PropertyName = propertyName;
        }

        public abstract void Invoke(AstRecord astRecord);
    }
}
