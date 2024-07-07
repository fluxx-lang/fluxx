

/**
 * @author Bret Johnson
 * @since 4/4/2015
 */

using Faml.Syntax;

namespace Faml.Interpreter
{
    public sealed class FunctionInvocationBooleanEval : BooleanEval
    {
        internal Eval[] Arguments;
        internal BooleanEval Expression;

        public FunctionInvocationBooleanEval(Eval[] arguments, CreateEvals createEvals, FunctionDefinitionSyntax functionDefinition)
        {
            this.Arguments = arguments;

            createEvals.DelayResolveFunctionEval(functionDefinition,
                (functionEval) => this.Expression = (BooleanEval)functionEval);
        }

        public override bool Eval()
        {
            int argumentsLength = this.Arguments.Length;
            for (int i = 0; i < argumentsLength; i++)
            {
                this.Arguments[i].Push();
            }

            int savedBaseIndex = Context.BaseIndex;
            Context.BaseIndex = Context.StackIndex;

            bool returnValue = this.Expression.Eval();

            Context.BaseIndex = savedBaseIndex;
            Context.StackIndex -= argumentsLength;

            return returnValue;
        }
    }

}
