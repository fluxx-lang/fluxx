/**
 * @author Bret Johnson
 * @since 4/5/2015
 */
using Fluxx.Api;
using Fluxx.Binding;
using Fluxx.CodeAnalysis.Text;

namespace Fluxx.Syntax.Type
{
    public class InvalidTypeReferenceSyntax : TypeReferenceSyntax
    {
        private readonly TypeBinding typeBinding = new InvalidTypeBinding(new QualifiableName("INVALID_TYPE"));

        public InvalidTypeReferenceSyntax() : base(TextSpanExtensions.NullTextSpan) {}

        public override TypeBinding GetTypeBinding()
        {
            return this.typeBinding;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.InvalidTypeReference;

        public override void WriteSource(SourceWriter sourceWriter)
        {
            sourceWriter.Write("<invalid-type>");
        }
    }
}
