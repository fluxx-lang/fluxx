using TypeTooling.CodeGeneration.Expressions;
using TypeTooling.RawTypes;

namespace TypeTooling.CodeGeneration
{
    public class MethodCallCode : ExpressionCode
    {
        private readonly ExpressionCode? _object;
        private readonly RawMethod _rawMethod;
        private readonly ExpressionCode[] _arguments;


        public MethodCallCode(ExpressionCode? objectCode, RawMethod rawMethod, params ExpressionCode[] arguments)
        {
            _object = objectCode;
            _rawMethod = rawMethod;
            _arguments = arguments;
        }

        public ExpressionCode? ObjectCode => _object;

        public RawMethod RawMethod => _rawMethod;

        public ExpressionCode[] Arguments => _arguments;
    }
}
