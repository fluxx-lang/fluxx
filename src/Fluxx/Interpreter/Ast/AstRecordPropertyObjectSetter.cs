namespace Faml.Interpreter.Ast
{
    public class AstRecordPropertyObjectSetter : AstRecordPropertySetter
    {
        private readonly ObjectEval objectEval;

        public AstRecordPropertyObjectSetter(string propertyName, ObjectEval objectEval) : base(propertyName)
        {
            this.objectEval = objectEval;
        }

        public override void Invoke(AstRecord astRecord)
        {
            astRecord.SetProperty(this.PropertyName, this.objectEval.Eval());
        }
    }
}
