using Faml.Syntax;

namespace Faml.Interpreter
{
    public sealed class FunctionInvocationDoubleEval : DoubleEval
    {
        internal readonly Eval[] Arguments;
        internal DoubleEval Expression;

        public FunctionInvocationDoubleEval(Eval[] arguments, CreateEvals createEvals, FunctionDefinitionSyntax functionDefinition)
        {
            this.Arguments = arguments;

            createEvals.DelayResolveFunctionEval(functionDefinition,
                (functionEval) => this.Expression = (DoubleEval)functionEval);
        }

        public override double Eval()
        {
            int argumentsLength = this.Arguments.Length;
            for (int i = 0; i < argumentsLength; i++)
            {
                this.Arguments[i].Push();
            }

            int savedBaseIndex = Context.BaseIndex;
            Context.BaseIndex = Context.StackIndex;

            double returnValue = this.Expression.Eval();

            Context.BaseIndex = savedBaseIndex;
            Context.StackIndex -= argumentsLength;

            return returnValue;
        }
    }
}
