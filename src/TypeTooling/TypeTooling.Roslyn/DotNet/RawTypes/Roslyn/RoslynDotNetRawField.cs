using Microsoft.CodeAnalysis;

namespace TypeTooling.DotNet.RawTypes.Roslyn {
    public class RoslynDotNetRawField : DotNetRawField {
        private readonly IFieldSymbol _fieldSymbol;

        public RoslynDotNetRawField(IFieldSymbol fieldSymbol) {
            _fieldSymbol = fieldSymbol;
        }

        public override string Name => _fieldSymbol.Name;

        public override DotNetRawType FieldType => new RoslynDotNetRawType(_fieldSymbol.Type);

        public override bool IsStatic => _fieldSymbol.IsStatic;

        public override bool IsPublic => _fieldSymbol.DeclaredAccessibility == Accessibility.Public;
    }
}
