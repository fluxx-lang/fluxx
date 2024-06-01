using System.Text;
using Microsoft.CodeAnalysis;

namespace TypeTooling.DotNet.RawTypes.Roslyn {
    public static class RoslynExtensions {
        private static readonly SymbolDisplayFormat Format = new SymbolDisplayFormat(
            SymbolDisplayGlobalNamespaceStyle.Omitted,
            SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.ExpandNullable);

        public static string GetFullName(this ITypeSymbol typeSymbol) {
            string fullName = typeSymbol.ToDisplayString(Format);
            if (typeSymbol is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.IsGenericType)
                return $"{fullName}`{namedTypeSymbol.Arity}";

            return fullName;
        }
    }
}
