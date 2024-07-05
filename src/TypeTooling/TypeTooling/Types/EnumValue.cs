using TypeTooling.CodeGeneration;

namespace TypeTooling.Types
{
    public class EnumValue
    {
        private readonly string _name;
        private readonly ExpressionAndHelpersCode _expressionAndHelpersCode;

        public EnumValue(string name, ExpressionAndHelpersCode expressionAndHelpersCode)
        {
            this._name = name;
            this._expressionAndHelpersCode = expressionAndHelpersCode;
        }

        public string Name => this._name;

        public ExpressionAndHelpersCode ExpressionAndHelpersCode => this._expressionAndHelpersCode;
    }
}
