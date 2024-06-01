using TypeTooling.Types;

/**
 * @author Bret Johnson
 * @since 4/4/2015
 */
namespace Faml.Interpreter.External {
    public sealed class ExternalPropertyAccessEval : ObjectEval {
        private readonly ObjectEval _objectEval;
        private readonly ObjectPropertyReader _objectPropertyReader;


        public ExternalPropertyAccessEval(ObjectEval objectEval, ObjectType objectType, ObjectProperty objectProperty) {
            _objectEval = objectEval;

            _objectPropertyReader = objectType.GetPropertyReader(objectProperty);
        }

        public override object Eval() {
            object obj = _objectEval.Eval();
            return _objectPropertyReader.Get(obj);
        }
    }
}
