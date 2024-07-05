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
            this._object = objectCode;
            this._rawMethod = rawMethod;
            this._arguments = arguments;
        }

        public ExpressionCode? ObjectCode => this._object;

        public RawMethod RawMethod => this._rawMethod;

        public ExpressionCode[] Arguments => this._arguments;
    }
}
