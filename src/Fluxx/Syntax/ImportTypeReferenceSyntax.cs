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

namespace Faml.Syntax {
    public sealed class ImportTypeReferenceSyntax : SyntaxNode {
        private readonly NameSyntax _nameSyntax;
        private TypeBinding? _typeBinding;
        private AttachedTypeBinding? _attachedTypeBinding;


        public ImportTypeReferenceSyntax(TextSpan span, NameSyntax nameSyntax) : base(span) {
            _nameSyntax = nameSyntax;
            _nameSyntax.SetParent(this);
        }

        public NameSyntax NameSyntax => _nameSyntax;

        public Name Name => _nameSyntax.Name;

        public QualifiableName QualifiedName => new QualifiableName(((ImportSyntax) Parent).Qualifier, Name);

        public TypeBinding GetTypeBinding() {
            if (_typeBinding == null)
                throw new Exception("TypeBinding not set for import; it hasn't been resolved yet");
            return _typeBinding;
        }

        public AttachedTypeBinding GetAttachedTypeBinding() {
            return _attachedTypeBinding;
        }

        public override void VisitChildren(SyntaxVisitor visitor) {
            visitor(_nameSyntax);
        }

        public override bool IsTerminalNode() {
            return false;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.ImportReference;

        public override void WriteSource(SourceWriter sourceWriter) {
            sourceWriter.Write(_nameSyntax);
        }

        // TODO: Fix this up to use bindingResolver, in some form
        protected internal override void ResolveExplicitTypeBindings(BindingResolver bindingResolver) {
            QualifiableName className = this.QualifiedName;
            FamlProject project = GetProject();

            TypeBindingResult typeBindingResult = project.ResolveTypeBinding(className);
            if (typeBindingResult is TypeBindingResult.Success success)
                _typeBinding = success.TypeBinding;
            else {
                this.AddError(
                    typeBindingResult.GetNotFoundOrOtherErrorMessage(
                        $"Type '{className}' not found in any of the provided libraries"));
                _typeBinding = InvalidTypeBinding.Instance;
            }

            _attachedTypeBinding = project.ResolveAttachedTypeBinding(className);
        }
    }
}
