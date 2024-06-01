using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using TypeTooling.DotNet.RawTypes.Reflection;

namespace TypeTooling.DotNet.RawTypes.Roslyn {
    public class RoslynDotNetRawMethod : DotNetRawMethod {
        private readonly IMethodSymbol _methodSymbol;

        public RoslynDotNetRawMethod(IMethodSymbol methodSymbol) {
            _methodSymbol = methodSymbol;
        }

        public override DotNetRawParameter[] GetParameters() {
            return ToParameterDescriptors(_methodSymbol.Parameters);
        }

        public static DotNetRawParameter[] ToParameterDescriptors(ImmutableArray<IParameterSymbol> parameters) {
            var rawParameter = new DotNetRawParameter[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
                rawParameter[i] = new RoslynDotNetRawParameter(parameters[i]);
            return rawParameter;
        }

        public override DotNetRawType ReturnType => new RoslynDotNetRawType(_methodSymbol.ReturnType);

        public override string Name => _methodSymbol.Name;

        public override bool IsStatic => _methodSymbol.IsStatic;

        public IMethodSymbol MethodSymbol => _methodSymbol;
    }
}
