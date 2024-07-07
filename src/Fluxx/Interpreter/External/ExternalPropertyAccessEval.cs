using TypeTooling.Types;

namespace Faml.Interpreter.External
{
    public sealed class ExternalPropertyAccessEval : ObjectEval
    {
        private readonly ObjectEval objectEval;
        private readonly ObjectPropertyReader objectPropertyReader;

        public ExternalPropertyAccessEval(ObjectEval objectEval, ObjectType objectType, ObjectProperty objectProperty)
        {
            this.objectEval = objectEval;

            this.objectPropertyReader = objectType.GetPropertyReader(objectProperty);
        }

        public override object Eval()
        {
            object obj = this.objectEval.Eval();
            return this.objectPropertyReader.Get(obj);
        }
    }
}
