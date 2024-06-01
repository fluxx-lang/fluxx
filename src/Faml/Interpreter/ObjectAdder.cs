/**
 * An Adder evaluates the expression and returns the result(s) by adding them to the prevailing adder, instead of
 * returning them as eval() method return value.
 *
 * @author Bret Johnson
 * @since 4/4/2015
 */
namespace Faml.Interpreter {
    public abstract class ObjectAdder : Adder {
        private ObjectEval _objectEval;

        public ObjectAdder(ObjectEval objectEval) {
            _objectEval = objectEval;
        }

        public abstract void Add(object @object);
    }
}
