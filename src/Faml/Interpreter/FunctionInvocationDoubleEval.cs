/**
 * @author Bret Johnson
 * @since 4/4/2015
 */

using Faml.Syntax;

namespace Faml.Interpreter {
    public sealed class FunctionInvocationDoubleEval : DoubleEval {
        internal readonly Eval[] Arguments;
        internal DoubleEval Expression;

        public FunctionInvocationDoubleEval(Eval[] arguments, CreateEvals createEvals, FunctionDefinitionSyntax functionDefinition) {
            Arguments = arguments;

            createEvals.DelayResolveFunctionEval(functionDefinition,
                (functionEval) => Expression = (DoubleEval)functionEval);
        }

        public override double Eval() {
            int argumentsLength = Arguments.Length;
            for (int i = 0; i < argumentsLength; i++)
                Arguments[i].Push();

            int savedBaseIndex = Context.BaseIndex;
            Context.BaseIndex = Context.StackIndex;

            double returnValue = Expression.Eval();

            Context.BaseIndex = savedBaseIndex;
            Context.StackIndex -= argumentsLength;

            return returnValue;
        }
    }
}
