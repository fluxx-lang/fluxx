using System.Collections.Immutable;
using TypeTooling.CodeGeneration.Expressions;

namespace TypeTooling.CodeGeneration
{
    public class FunctionDelegateInvokeCode : ExpressionCode
    {
        private readonly ExpressionCode _functionDelegate;
        private readonly ImmutableArray<ExpressionCode> _arguments;


        public FunctionDelegateInvokeCode(ExpressionCode functionDelegate, ImmutableArray<ExpressionCode> arguments)
        {
            this._functionDelegate = functionDelegate;
            this._arguments = arguments;
        }

        public ExpressionCode FunctionDelegate => this._functionDelegate;

        public ImmutableArray<ExpressionCode> Arguments => this._arguments;
    }
}
