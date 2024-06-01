/**
 * @author Bret Johnson
 * @since 4/5/2015
 */

using Faml.Api;
using Faml.Binding;
using Faml.CodeAnalysis.Text;

namespace Faml.Syntax.Type {
    public class InvalidTypeReferenceSyntax : TypeReferenceSyntax {
        private readonly TypeBinding _typeBinding = new InvalidTypeBinding(new QualifiableName("INVALID_TYPE"));

        public InvalidTypeReferenceSyntax() : base(TextSpanExtensions.NullTextSpan) {}

        public override TypeBinding GetTypeBinding() {
            return _typeBinding;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.InvalidTypeReference;

        public override void WriteSource(SourceWriter sourceWriter) {
            sourceWriter.Write("<invalid-type>");
        }
    }
}
