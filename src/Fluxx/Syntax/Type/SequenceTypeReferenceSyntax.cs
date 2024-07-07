/**
 * @author Bret Johnson
 * @since 6/5/2015
 */

using Faml.Binding;
using Faml.Binding.Resolver;
using Faml.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax.Type
{
    public sealed class SequenceTypeReferenceSyntax : TypeReferenceSyntax
    {
        private readonly TypeReferenceSyntax _elementTypeReferenceSyntax;
        private TypeBinding _typeBinding;

        public SequenceTypeReferenceSyntax(TextSpan span, TypeReferenceSyntax elementTypeReferenceSyntax) : base(span)
        {
            this._elementTypeReferenceSyntax = elementTypeReferenceSyntax;
            this._elementTypeReferenceSyntax.SetParent(this);
        }

        public override bool IsTerminalNode()
        {
            return false;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.SequenceTypeReference;

        public override void VisitChildren(SyntaxVisitor visitor)
        {
            visitor(this._elementTypeReferenceSyntax);
        }

        protected internal override void ResolveExplicitTypeBindings(BindingResolver bindingResolver)
        {
            this._typeBinding = new SequenceTypeBinding(this._elementTypeReferenceSyntax.GetTypeBinding());
        }

        public override TypeBinding GetTypeBinding()
        {
            return this._typeBinding;
        }
                                     
        public override void WriteSource(SourceWriter sourceWriter)
        {
            sourceWriter.Write(this._elementTypeReferenceSyntax);
            sourceWriter.Write("...");
        }
    }
}
