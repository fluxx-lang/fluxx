using Fluxx.Binding;
using Fluxx.Binding.Resolver;
using Fluxx.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

namespace Fluxx.Syntax.Type
{
    public sealed class SequenceTypeReferenceSyntax : TypeReferenceSyntax
    {
        private readonly TypeReferenceSyntax elementTypeReferenceSyntax;
        private TypeBinding typeBinding;

        public SequenceTypeReferenceSyntax(TextSpan span, TypeReferenceSyntax elementTypeReferenceSyntax) : base(span)
        {
            this.elementTypeReferenceSyntax = elementTypeReferenceSyntax;
            this.elementTypeReferenceSyntax.SetParent(this);
        }

        public override bool IsTerminalNode()
        {
            return false;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.SequenceTypeReference;

        public override void VisitChildren(SyntaxVisitor visitor)
        {
            visitor(this.elementTypeReferenceSyntax);
        }

        protected internal override void ResolveExplicitTypeBindings(BindingResolver bindingResolver)
        {
            this.typeBinding = new SequenceTypeBinding(this.elementTypeReferenceSyntax.GetTypeBinding());
        }

        public override TypeBinding GetTypeBinding()
        {
            return this.typeBinding;
        }
                                     
        public override void WriteSource(SourceWriter sourceWriter)
        {
            sourceWriter.Write(this.elementTypeReferenceSyntax);
            sourceWriter.Write("...");
        }
    }
}
