using Microsoft.CodeAnalysis;

namespace TypeTooling.DotNet.RawTypes.Roslyn {
    public class RoslynDotNetRawParameter : DotNetRawParameter {
        private readonly IParameterSymbol _parameterSymbol;

        public RoslynDotNetRawParameter(IParameterSymbol parameterSymbol) {
            _parameterSymbol = parameterSymbol;
        }

        public override string Name => _parameterSymbol.Name;

        public override DotNetRawType ParameterType => new RoslynDotNetRawType(_parameterSymbol.Type);

        public IParameterSymbol ParameterSymbol => _parameterSymbol;
    }
}
