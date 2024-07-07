using Fluxx.Syntax;

namespace Fluxx.Interpreter
{
    public sealed class FunctionInvocationIntEval : IntEval
    {
        internal readonly Eval[] Arguments;
        internal IntEval Expression;

        public FunctionInvocationIntEval(Eval[] arguments, CreateEvals createEvals, FunctionDefinitionSyntax functionDefinition)
        {
            this.Arguments = arguments;

            createEvals.DelayResolveFunctionEval(functionDefinition,
                (functionEval) => this.Expression = (IntEval)functionEval);
        }

        public override int Eval()
        {
            int argumentsLength = this.Arguments.Length;
            for (int i = 0; i < argumentsLength; i++)
            {
                this.Arguments[i].Push();
            }

            int savedBaseIndex = Context.BaseIndex;
            Context.BaseIndex = Context.StackIndex;

            int returnValue = this.Expression.Eval();

            Context.BaseIndex = savedBaseIndex;
            Context.StackIndex -= argumentsLength;

            return returnValue;
        }
    }
}
