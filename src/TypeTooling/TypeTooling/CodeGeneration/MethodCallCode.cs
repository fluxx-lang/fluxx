using TypeTooling.CodeGeneration.Expressions;
using TypeTooling.RawTypes;

namespace TypeTooling.CodeGeneration
{
    public class MethodCallCode : ExpressionCode
    {
        private readonly ExpressionCode? @object;
        private readonly RawMethod rawMethod;
        private readonly ExpressionCode[] arguments;

        public MethodCallCode(ExpressionCode? objectCode, RawMethod rawMethod, params ExpressionCode[] arguments)
        {
            this.@object = objectCode;
            this.rawMethod = rawMethod;
            this.arguments = arguments;
        }

        public ExpressionCode? ObjectCode => this.@object;

        public RawMethod RawMethod => this.rawMethod;

        public ExpressionCode[] Arguments => this.arguments;
    }
}
