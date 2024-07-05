using TypeTooling.CodeGeneration;

namespace TypeTooling.Types
{
    public class EnumValue
    {
        private readonly string name;
        private readonly ExpressionAndHelpersCode expressionAndHelpersCode;

        public EnumValue(string name, ExpressionAndHelpersCode expressionAndHelpersCode)
        {
            this.name = name;
            this.expressionAndHelpersCode = expressionAndHelpersCode;
        }

        public string Name => this.name;

        public ExpressionAndHelpersCode ExpressionAndHelpersCode => this.expressionAndHelpersCode;
    }
}
