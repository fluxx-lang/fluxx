namespace Faml.Interpreter.Ast
{
    public class AstRecordPropertyBooleanSetter : AstRecordPropertySetter
    {
        private readonly BooleanEval booleanEval;

        public AstRecordPropertyBooleanSetter(string propertyName, BooleanEval booleanEval) : base(propertyName)
        {
            this.booleanEval = booleanEval;
        }

        public override void Invoke(AstRecord astRecord)
        {
            astRecord.SetProperty(this.PropertyName, this.booleanEval.Eval());
        }
    }
}
