/**
 * @author Bret Johnson
 * @since 6/5/2015
 */

using Faml.Api;
using Faml.Binding;
using Faml.Binding.Resolver;
using Faml.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax.Type {
    public sealed class ObjectTypeReferenceSyntax : TypeReferenceSyntax {
        private readonly QualifiableNameSyntax _typeNameSyntax;
        private TypeBinding _typeBinding;

        public ObjectTypeReferenceSyntax(TextSpan span, QualifiableNameSyntax typeNameSyntax) : base(span) {
            this._typeNameSyntax = typeNameSyntax;
            this._typeNameSyntax.SetParent(this);
        }

        public QualifiableNameSyntax TypeNameSyntax => this._typeNameSyntax;

        public QualifiableName TypeName => this._typeNameSyntax.Name;

        public override bool IsTerminalNode() {
            return false;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.ObjectTypeReference;

        public override void VisitChildren(SyntaxNode.SyntaxVisitor visitor) {
            visitor(this._typeNameSyntax);
        }

        protected internal override void ResolveExplicitTypeBindings(BindingResolver bindingResolver) {
            this._typeBinding = bindingResolver.ResolveObjectTypeBinding(this);
        }

        public override TypeBinding GetTypeBinding() {
            return this._typeBinding;
        }

        public override void WriteSource(SourceWriter sourceWriter) {
            sourceWriter.Write(this._typeNameSyntax);
        }
    }
}
