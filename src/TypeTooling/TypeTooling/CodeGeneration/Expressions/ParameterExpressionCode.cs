using TypeTooling.RawTypes;

namespace TypeTooling.CodeGeneration.Expressions
{
    public class ParameterExpressionCode(string name, RawType type) : ExpressionCode
    {
        public string Name { get; } = name;

        public RawType Type { get; } = type;
    }
}
