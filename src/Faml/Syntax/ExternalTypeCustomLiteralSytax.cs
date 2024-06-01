using Microsoft.CodeAnalysisP.Text;
using Faml.Binding;
using Faml.Binding.External;
using Faml.CodeAnalysis.Text;
using Faml.Syntax.Expression;
using TypeTooling.Types;

namespace Faml.Syntax {
    public class ExternalTypeCustomLiteralSytax : ExpressionSyntax {
        private readonly ExternalObjectTypeBinding _propertyTypeBinding;
        private readonly TypeToolingType _externalType;
        private readonly string _literalSource;
        private readonly CustomLiteral _customLiteral;


        public ExternalTypeCustomLiteralSytax(TextSpan span, ExternalObjectTypeBinding propertyTypeBinding, TypeToolingType externalType, string literalSource, CustomLiteral customLiteral) : base(span) {
            _propertyTypeBinding = propertyTypeBinding;
            _externalType = externalType;
            _literalSource = literalSource;
            _customLiteral = customLiteral;
        }

        public override TypeBinding GetTypeBinding() {
            return _propertyTypeBinding;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.ExternalTypeLiteral;

        public TypeToolingType ExternalType => _externalType;

        public string LiteralSource => _literalSource;

        public CustomLiteral CustomLiteral => _customLiteral;

        public override void WriteSource(SourceWriter sourceWriter) {
            sourceWriter.Write(_literalSource);
        }
    }
}
