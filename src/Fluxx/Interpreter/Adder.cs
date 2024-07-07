/**
 * An Adder evaluates the expression and returns the result(s) by adding them to the prevailing adder, instead of
 *  returning them as eval() method return value.
 *
 * @author Bret Johnson
 * @since 4/4/2015
 */
namespace Fluxx.Interpreter
{
    public abstract class Adder
    {
        public abstract void EvalAndAdd();
    }
}
