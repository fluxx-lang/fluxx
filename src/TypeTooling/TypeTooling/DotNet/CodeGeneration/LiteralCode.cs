using TypeTooling.CodeGeneration.Expressions;

namespace TypeTooling.DotNet.CodeGeneration
{
    public class FunctionDelegateHolderCode : ExpressionCode
    {
        public FunctionDelegateHolder FunctionDelegateHolder { get; }

        public FunctionDelegateHolderCode(FunctionDelegateHolder functionDelegateHolder)
        {
            this.FunctionDelegateHolder = functionDelegateHolder;
        }
    }
}
