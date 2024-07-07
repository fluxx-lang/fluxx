using System;
using Faml.Api;
using Faml.Binding;
using Faml.Binding.Resolver;
using Faml.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

/**
 * @author Bret Johnson
 * @since 6/6/2015
 */
namespace Faml.Syntax
{
    public sealed class ImportTypeReferenceSyntax : SyntaxNode
    {
        private readonly NameSyntax nameSyntax;
        private TypeBinding? typeBinding;
        private AttachedTypeBinding? attachedTypeBinding;

        public ImportTypeReferenceSyntax(TextSpan span, NameSyntax nameSyntax) : base(span)
        {
            this.nameSyntax = nameSyntax;
            this.nameSyntax.SetParent(this);
        }

        public NameSyntax NameSyntax => this.nameSyntax;

        public Name Name => this.nameSyntax.Name;

        public QualifiableName QualifiedName => new QualifiableName(((ImportSyntax)this.Parent).Qualifier, this.Name);

        public TypeBinding GetTypeBinding()
        {
            if (this.typeBinding == null)
            {
                throw new Exception("TypeBinding not set for import; it hasn't been resolved yet");
            }

            return this.typeBinding;
        }

        public AttachedTypeBinding GetAttachedTypeBinding()
        {
            return this.attachedTypeBinding;
        }

        public override void VisitChildren(SyntaxVisitor visitor)
        {
            visitor(this.nameSyntax);
        }

        public override bool IsTerminalNode()
        {
            return false;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.ImportReference;

        public override void WriteSource(SourceWriter sourceWriter)
        {
            sourceWriter.Write(this.nameSyntax);
        }

        // TODO: Fix this up to use bindingResolver, in some form
        protected internal override void ResolveExplicitTypeBindings(BindingResolver bindingResolver)
        {
            QualifiableName className = this.QualifiedName;
            FamlProject project = this.GetProject();

            TypeBindingResult typeBindingResult = project.ResolveTypeBinding(className);
            if (typeBindingResult is TypeBindingResult.Success success)
            {
                this.typeBinding = success.TypeBinding;
            }
            else
            {
                this.AddError(
                    typeBindingResult.GetNotFoundOrOtherErrorMessage(
                        $"Type '{className}' not found in any of the provided libraries"));
                this.typeBinding = InvalidTypeBinding.Instance;
            }

            this.attachedTypeBinding = project.ResolveAttachedTypeBinding(className);
        }
    }
}
