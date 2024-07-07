namespace Fluxx.Interpreter.Ast
{
    public abstract class AstRecordPropertySetter
    {
        protected internal readonly string PropertyName;

        protected AstRecordPropertySetter(string propertyName)
        {
            this.PropertyName = propertyName;
        }

        public abstract void Invoke(AstRecord astRecord);
    }
}
