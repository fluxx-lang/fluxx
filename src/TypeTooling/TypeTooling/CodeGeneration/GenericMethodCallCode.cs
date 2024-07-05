using TypeTooling.CodeGeneration.Expressions;
using TypeTooling.RawTypes;

namespace TypeTooling.CodeGeneration
{
    public class GenericMethodCallCode : ExpressionCode
    {
        private readonly RawType[] _genericTypeArguments;
        private readonly ExpressionCode? _object;
        private readonly RawMethod _rawMethod;
        private readonly ExpressionCode[] _arguments;


        public GenericMethodCallCode(RawType[] genericTypeArguments, ExpressionCode? objectCode, RawMethod rawMethod, params ExpressionCode[] arguments)
        {
            _genericTypeArguments = genericTypeArguments;
            _object = objectCode;
            _rawMethod = rawMethod;
            _arguments = arguments;
        }

        public RawType[] GenericTypeArguments => _genericTypeArguments;

        public ExpressionCode? ObjectCode => _object;

        public RawMethod RawMethod => _rawMethod;

        public ExpressionCode[] Arguments => _arguments;
    }
}
