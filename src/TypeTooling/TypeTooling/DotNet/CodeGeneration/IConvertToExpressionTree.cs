using System.Linq.Expressions;
using TypeTooling.CodeGeneration.Expressions;

namespace TypeTooling.DotNet.CodeGeneration {
    public interface IConvertToExpressionTree {
        LambdaExpression Result { get; }

        Expression ConvertExpression(ExpressionCode expressionCode);
    }
}