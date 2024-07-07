using Faml.Binding;
using Faml.Binding.External;
using Faml.CodeAnalysis.Text;
using Faml.Syntax.Expression;
using Microsoft.CodeAnalysis.Text;
using TypeTooling.Types;

namespace Faml.Syntax
{
    public class ExternalTypeCustomLiteralSytax : ExpressionSyntax
    {
        private readonly ExternalObjectTypeBinding _propertyTypeBinding;
        private readonly TypeToolingType _externalType;
        private readonly string _literalSource;
        private readonly CustomLiteral _customLiteral;


        public ExternalTypeCustomLiteralSytax(TextSpan span, ExternalObjectTypeBinding propertyTypeBinding, TypeToolingType externalType, string literalSource, CustomLiteral customLiteral) : base(span)
        {
            this._propertyTypeBinding = propertyTypeBinding;
            this._externalType = externalType;
            this._literalSource = literalSource;
            this._customLiteral = customLiteral;
        }

        public override TypeBinding GetTypeBinding()
        {
            return this._propertyTypeBinding;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.ExternalTypeLiteral;

        public TypeToolingType ExternalType => this._externalType;

        public string LiteralSource => this._literalSource;

        public CustomLiteral CustomLiteral => this._customLiteral;

        public override void WriteSource(SourceWriter sourceWriter)
        {
            sourceWriter.Write(this._literalSource);
        }
    }
}
