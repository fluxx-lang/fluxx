using Microsoft.CodeAnalysis;

namespace TypeTooling.DotNet.RawTypes.Roslyn {
    public class RoslynDotNetRawConstructor : DotNetRawConstructor {
        private readonly IMethodSymbol _methodSymbol;

        public RoslynDotNetRawConstructor(IMethodSymbol methodSymbol) {
            _methodSymbol = methodSymbol;
        }

        public override DotNetRawParameter[] GetParameters() {
            return RoslynDotNetRawMethod.ToParameterDescriptors(_methodSymbol.Parameters);
        }

        public IMethodSymbol MethodSymbol => _methodSymbol;
    }
}
