using TypeTooling.CodeGeneration.Expressions;
using TypeTooling.RawTypes;

namespace TypeTooling.CodeGeneration
{
    public class GenericMethodCallCode : ExpressionCode
    {
        private readonly RawType[] genericTypeArguments;
        private readonly ExpressionCode? @object;
        private readonly RawMethod rawMethod;
        private readonly ExpressionCode[] arguments;


        public GenericMethodCallCode(RawType[] genericTypeArguments, ExpressionCode? objectCode, RawMethod rawMethod, params ExpressionCode[] arguments)
        {
            this.genericTypeArguments = genericTypeArguments;
            this.@object = objectCode;
            this.rawMethod = rawMethod;
            this.arguments = arguments;
        }

        public RawType[] GenericTypeArguments => this.genericTypeArguments;

        public ExpressionCode? ObjectCode => this.@object;

        public RawMethod RawMethod => this.rawMethod;

        public ExpressionCode[] Arguments => this.arguments;
    }
}
