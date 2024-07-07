using Fluxx.Binding;
using Fluxx.Binding.External;
using Fluxx.CodeAnalysis.Text;
using Fluxx.Syntax.Expression;
using Microsoft.CodeAnalysis.Text;
using TypeTooling.Types;

namespace Fluxx.Syntax
{
    public class ExternalTypeCustomLiteralSyntax : ExpressionSyntax
    {
        private readonly ExternalObjectTypeBinding propertyTypeBinding;
        private readonly TypeToolingType externalType;
        private readonly string literalSource;
        private readonly CustomLiteral customLiteral;

        public ExternalTypeCustomLiteralSyntax(TextSpan span, ExternalObjectTypeBinding propertyTypeBinding, TypeToolingType externalType, string literalSource, CustomLiteral customLiteral) : base(span)
        {
            this.propertyTypeBinding = propertyTypeBinding;
            this.externalType = externalType;
            this.literalSource = literalSource;
            this.customLiteral = customLiteral;
        }

        public override TypeBinding GetTypeBinding()
        {
            return this.propertyTypeBinding;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.ExternalTypeLiteral;

        public TypeToolingType ExternalType => this.externalType;

        public string LiteralSource => this.literalSource;

        public CustomLiteral CustomLiteral => this.customLiteral;

        public override void WriteSource(SourceWriter sourceWriter)
        {
            sourceWriter.Write(this.literalSource);
        }
    }
}
