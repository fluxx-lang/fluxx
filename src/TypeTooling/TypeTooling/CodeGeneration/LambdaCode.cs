using System.Collections.Immutable;
using TypeTooling.CodeGeneration.Expressions;

namespace TypeTooling.CodeGeneration
{
    public class LambdaCode
    {
        /// <summary>
        /// The Name may be used for debug information.
        /// </summary>
        public string Name { get; }
        public ImmutableArray<ParameterExpressionCode> Parameters { get; }
        public ExpressionCode Body { get; }

        public LambdaCode(string name, ImmutableArray<ParameterExpressionCode> parameters, ExpressionCode body)
        {
            Name = name;
            Parameters = parameters;
            Body = body;
        }
    }
}
