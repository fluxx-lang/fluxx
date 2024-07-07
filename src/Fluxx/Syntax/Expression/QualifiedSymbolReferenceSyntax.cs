using System;
using Faml.Api;
using Faml.Binding;
using Faml.Binding.Resolver;
using Faml.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax.Expression
{
    public sealed class QualifiedSymbolReferenceSyntax : ExpressionSyntax
    {
        private readonly QualifiedSymbolReferenceSyntax? _qualifier;
        private readonly NameSyntax _symbol;
        private readonly QualifiableName _qualifiableName;
        private TypeBinding? _typeBinding;
        private PropertyBinding? _propertyBinding;


        public QualifiedSymbolReferenceSyntax(TextSpan span, QualifiedSymbolReferenceSyntax? qualifier, NameSyntax symbol) : base(span)
        {
            this._qualifier = qualifier;
            this._qualifier?.SetParent(this);

            this._symbol = symbol;
            this._symbol.SetParent(this);

            Name symbolName = symbol.Name;
            this._qualifiableName = qualifier == null ? symbolName.ToQualifiableName() : new QualifiableName(qualifier.QualifiableName, symbolName);
        }

        public QualifiableName QualifiableName => this._qualifiableName;

        public bool IsType => this._typeBinding != null;

        protected internal override void ResolveBindings(BindingResolver bindingResolver)
        {
            if (this._qualifier != null && this._qualifier.IsType)
            {
                TypeBinding typeBinding = this._qualifier.GetTypeBinding();
                this._propertyBinding = bindingResolver.ResolvePropertyBinding(typeBinding, this._symbol);
            }
            else
            {
                QualifiableName qualifiableName = this.QualifiableName;
                TypeBindingResult typeBindingResult = bindingResolver.FindTypeBindingForType(qualifiableName);

                if (typeBindingResult is TypeBindingResult.Success successResult)
                    this._typeBinding = successResult.TypeBinding;
                else if (typeBindingResult is TypeBindingResult.Error errorResult)
                {
                    this.AddError(errorResult.Message);
                    this._typeBinding = new InvalidTypeBinding(qualifiableName);
                }
                else
                {
                    // If the type wasn't found, then don't set any binding
                }
            }
        }

        public override TypeBinding GetTypeBinding()
        {
            if (this.IsType)
            {
                return this._typeBinding;
            }

            if (this._propertyBinding != null)
            {
                return this._propertyBinding.GetTypeBinding();
            }

            throw new Exception(
                $"Can't call GetTypeBinding on symbol reference that isn't a type; check IsType first");
        }

        public override bool IsTerminalNode() { return false; }

        public override SyntaxNodeType NodeType => SyntaxNodeType.SymbolReference;

        public override void VisitChildren(SyntaxNode.SyntaxVisitor visitor)
        {
            if (this._qualifier != null)
            {
                visitor(this._qualifier);
            }

            visitor(this._symbol);
        }

        public override void WriteSource(SourceWriter sourceWriter)
        {
            if (this._qualifier != null)
            {
                this._qualifier.WriteSource(sourceWriter);
                sourceWriter.Write(".");
            }
            sourceWriter.Write(this._symbol);
        }
    }
}
