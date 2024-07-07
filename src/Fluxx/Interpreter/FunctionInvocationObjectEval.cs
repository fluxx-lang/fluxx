

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
            this.Arguments = arguments;

            createEvals.DelayResolveFunctionEval(functionDefinition,
                (functionEval) => this.Expression = (ObjectEval)functionEval);
        }

        public override object Eval() {
            int argumentsLength = this.Arguments.Length;
            for (int i = 0; i < argumentsLength; i++)
                this.Arguments[i].Push();

            int savedBaseIndex = Context.BaseIndex;
            Context.BaseIndex = Context.StackIndex;

            object returnValue = this.Expression.Eval();

            Context.BaseIndex = savedBaseIndex;
            Context.StackIndex -= argumentsLength;

            return returnValue;
        }
    }
}
