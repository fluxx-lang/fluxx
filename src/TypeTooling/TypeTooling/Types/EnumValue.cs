using TypeTooling.CodeGeneration;

namespace TypeTooling.Types
{
    public class EnumValue
    {
        private readonly string _name;
        private readonly ExpressionAndHelpersCode _expressionAndHelpersCode;

        public EnumValue(string name, ExpressionAndHelpersCode expressionAndHelpersCode)
        {
            _name = name;
            _expressionAndHelpersCode = expressionAndHelpersCode;
        }

        public string Name => _name;

        public ExpressionAndHelpersCode ExpressionAndHelpersCode => _expressionAndHelpersCode;
    }
}
