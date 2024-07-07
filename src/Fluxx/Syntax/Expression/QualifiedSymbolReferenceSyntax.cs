using System;
using Faml.Api;
using Faml.Binding;
using Faml.Binding.Resolver;
using Faml.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax.Expression {
    public sealed class QualifiedSymbolReferenceSyntax : ExpressionSyntax {
        private readonly QualifiedSymbolReferenceSyntax? _qualifier;
        private readonly NameSyntax _symbol;
        private readonly QualifiableName _qualifiableName;
        private TypeBinding? _typeBinding;
        private PropertyBinding? _propertyBinding;


        public QualifiedSymbolReferenceSyntax(TextSpan span, QualifiedSymbolReferenceSyntax? qualifier, NameSyntax symbol) : base(span) {
            _qualifier = qualifier;
            _qualifier?.SetParent(this);

            _symbol = symbol;
            _symbol.SetParent(this);

            Name symbolName = symbol.Name;
            _qualifiableName = qualifier == null ? symbolName.ToQualifiableName() : new QualifiableName(qualifier.QualifiableName, symbolName);
        }

        public QualifiableName QualifiableName => _qualifiableName;

        public bool IsType => _typeBinding != null;

        protected internal override void ResolveBindings(BindingResolver bindingResolver) {
            if (_qualifier != null && _qualifier.IsType) {
                TypeBinding typeBinding = _qualifier.GetTypeBinding();
                _propertyBinding = bindingResolver.ResolvePropertyBinding(typeBinding, _symbol);
            }
            else {
                QualifiableName qualifiableName = QualifiableName;
                TypeBindingResult typeBindingResult = bindingResolver.FindTypeBindingForType(qualifiableName);

                if (typeBindingResult is TypeBindingResult.Success successResult)
                    _typeBinding = successResult.TypeBinding;
                else if (typeBindingResult is TypeBindingResult.Error errorResult) {
                    this.AddError(errorResult.Message);
                    _typeBinding = new InvalidTypeBinding(qualifiableName);
                }
                else {
                    // If the type wasn't found, then don't set any binding
                }
            }
        }

        public override TypeBinding GetTypeBinding() {
            if (IsType)
                return _typeBinding;
            if (_propertyBinding != null)
                return _propertyBinding.GetTypeBinding();
            throw new Exception(
                $"Can't call GetTypeBinding on symbol reference that isn't a type; check IsType first");
        }

        public override bool IsTerminalNode() { return false; }

        public override SyntaxNodeType NodeType => SyntaxNodeType.SymbolReference;

        public override void VisitChildren(SyntaxNode.SyntaxVisitor visitor) {
            if (_qualifier != null)
                visitor(_qualifier);
            visitor(_symbol);
        }

        public override void WriteSource(SourceWriter sourceWriter) {
            if (_qualifier != null) {
                _qualifier.WriteSource(sourceWriter);
                sourceWriter.Write(".");
            }
            sourceWriter.Write(_symbol);
        }
    }
}
