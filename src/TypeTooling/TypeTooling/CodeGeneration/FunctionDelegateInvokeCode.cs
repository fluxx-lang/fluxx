using System.Collections.Immutable;
using TypeTooling.CodeGeneration.Expressions;

namespace TypeTooling.CodeGeneration
{
    public class FunctionDelegateInvokeCode : ExpressionCode
    {
        private readonly ExpressionCode functionDelegate;
        private readonly ImmutableArray<ExpressionCode> arguments;

        public FunctionDelegateInvokeCode(ExpressionCode functionDelegate, ImmutableArray<ExpressionCode> arguments)
        {
            this.functionDelegate = functionDelegate;
            this.arguments = arguments;
        }

        public ExpressionCode FunctionDelegate => this.functionDelegate;

        public ImmutableArray<ExpressionCode> Arguments => this.arguments;
    }
}
