namespace Faml.Interpreter.Ast
{
    public class AstRecordPropertyIntSetter : AstRecordPropertySetter
    {
        private readonly IntEval intEval;

        public AstRecordPropertyIntSetter(string propertyName, IntEval intEval) : base(propertyName)
        {
            this.intEval = intEval;
        }

        public override void Invoke(AstRecord astRecord)
        {
            astRecord.SetProperty(this.PropertyName, this.intEval.Eval());
        }
    }
}
