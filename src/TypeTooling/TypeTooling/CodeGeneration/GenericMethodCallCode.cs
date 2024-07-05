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
            this._genericTypeArguments = genericTypeArguments;
            this._object = objectCode;
            this._rawMethod = rawMethod;
            this._arguments = arguments;
        }

        public RawType[] GenericTypeArguments => this._genericTypeArguments;

        public ExpressionCode? ObjectCode => this._object;

        public RawMethod RawMethod => this._rawMethod;

        public ExpressionCode[] Arguments => this._arguments;
    }
}
