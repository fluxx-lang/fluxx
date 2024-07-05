

/**
 * @author Bret Johnson
 * @since 4/4/2015
 */

using Faml.Syntax;

namespace Faml.Interpreter {
    public sealed class FunctionInvocationObjectEval : ObjectEval {
        internal Eval[] Arguments;
        internal ObjectEval Expression;

        public FunctionInvocationObjectEval(Eval[] arguments, CreateEvals createEvals, FunctionDefinitionSyntax functionDefinition) {
            Arguments = arguments;

            createEvals.DelayResolveFunctionEval(functionDefinition,
                (functionEval) => Expression = (ObjectEval)functionEval);
        }

        public override object Eval() {
            int argumentsLength = Arguments.Length;
            for (int i = 0; i < argumentsLength; i++)
                Arguments[i].Push();

            int savedBaseIndex = Context.BaseIndex;
            Context.BaseIndex = Context.StackIndex;

            object returnValue = Expression.Eval();

            Context.BaseIndex = savedBaseIndex;
            Context.StackIndex -= argumentsLength;

            return returnValue;
        }
    }
}
