/**
 * @author Bret Johnson
 * @since 6/5/2015
 */

using Faml.Api;
using Microsoft.CodeAnalysisP.Text;
using Faml.Binding;
using Faml.Binding.Resolver;
using Faml.CodeAnalysis.Text;

namespace Faml.Syntax.Type {
    public sealed class ObjectTypeReferenceSyntax : TypeReferenceSyntax {
        private readonly QualifiableNameSyntax _typeNameSyntax;
        private TypeBinding _typeBinding;

        public ObjectTypeReferenceSyntax(TextSpan span, QualifiableNameSyntax typeNameSyntax) : base(span) {
            _typeNameSyntax = typeNameSyntax;
            _typeNameSyntax.SetParent(this);
        }

        public QualifiableNameSyntax TypeNameSyntax => _typeNameSyntax;

        public QualifiableName TypeName => _typeNameSyntax.Name;

        public override bool IsTerminalNode() {
            return false;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.ObjectTypeReference;

        public override void VisitChildren(SyntaxNode.SyntaxVisitor visitor) {
            visitor(_typeNameSyntax);
        }

        protected internal override void ResolveExplicitTypeBindings(BindingResolver bindingResolver) {
            _typeBinding = bindingResolver.ResolveObjectTypeBinding(this);
        }

        public override TypeBinding GetTypeBinding() {
            return _typeBinding;
        }

        public override void WriteSource(SourceWriter sourceWriter) {
            sourceWriter.Write(_typeNameSyntax);
        }
    }
}
