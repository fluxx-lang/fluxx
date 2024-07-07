using Faml.Api;
using Faml.Binding;
using Faml.Binding.Resolver;
using Faml.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax.Type
{
    public sealed class ObjectTypeReferenceSyntax : TypeReferenceSyntax
    {
        private readonly QualifiableNameSyntax typeNameSyntax;
        private TypeBinding typeBinding;

        public ObjectTypeReferenceSyntax(TextSpan span, QualifiableNameSyntax typeNameSyntax) : base(span)
        {
            this.typeNameSyntax = typeNameSyntax;
            this.typeNameSyntax.SetParent(this);
        }

        public QualifiableNameSyntax TypeNameSyntax => this.typeNameSyntax;

        public QualifiableName TypeName => this.typeNameSyntax.Name;

        public override bool IsTerminalNode()
        {
            return false;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.ObjectTypeReference;

        public override void VisitChildren(SyntaxNode.SyntaxVisitor visitor)
        {
            visitor(this.typeNameSyntax);
        }

        protected internal override void ResolveExplicitTypeBindings(BindingResolver bindingResolver)
        {
            this.typeBinding = bindingResolver.ResolveObjectTypeBinding(this);
        }

        public override TypeBinding GetTypeBinding()
        {
            return this.typeBinding;
        }

        public override void WriteSource(SourceWriter sourceWriter)
        {
            sourceWriter.Write(this.typeNameSyntax);
        }
    }
}
