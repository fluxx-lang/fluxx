/**
 * @author Bret Johnson
 * @since 4/4/2015
 */

using Faml.Syntax;

namespace Faml.Interpreter {
    public sealed class FunctionInvocationIntEval : IntEval {
        internal readonly Eval[] Arguments;
        internal IntEval Expression;

        public FunctionInvocationIntEval(Eval[] arguments, CreateEvals createEvals, FunctionDefinitionSyntax functionDefinition) {
            Arguments = arguments;

            createEvals.DelayResolveFunctionEval(functionDefinition,
                (functionEval) => Expression = (IntEval)functionEval);
        }

        public override int Eval() {
            int argumentsLength = Arguments.Length;
            for (int i = 0; i < argumentsLength; i++)
                Arguments[i].Push();

            int savedBaseIndex = Context.BaseIndex;
            Context.BaseIndex = Context.StackIndex;

            int returnValue = Expression.Eval();

            Context.BaseIndex = savedBaseIndex;
            Context.StackIndex -= argumentsLength;

            return returnValue;
        }
    }
}
