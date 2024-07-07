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
        private readonly QualifiedSymbolReferenceSyntax? qualifier;
        private readonly NameSyntax symbol;
        private readonly QualifiableName qualifiableName;
        private TypeBinding? typeBinding;
        private PropertyBinding? propertyBinding;

        public QualifiedSymbolReferenceSyntax(TextSpan span, QualifiedSymbolReferenceSyntax? qualifier, NameSyntax symbol) : base(span)
        {
            this.qualifier = qualifier;
            this.qualifier?.SetParent(this);

            this.symbol = symbol;
            this.symbol.SetParent(this);

            Name symbolName = symbol.Name;
            this.qualifiableName = qualifier == null ? symbolName.ToQualifiableName() : new QualifiableName(qualifier.QualifiableName, symbolName);
        }

        public QualifiableName QualifiableName => this.qualifiableName;

        public bool IsType => this.typeBinding != null;

        protected internal override void ResolveBindings(BindingResolver bindingResolver)
        {
            if (this.qualifier != null && this.qualifier.IsType)
            {
                TypeBinding typeBinding = this.qualifier.GetTypeBinding();
                this.propertyBinding = bindingResolver.ResolvePropertyBinding(typeBinding, this.symbol);
            }
            else
            {
                QualifiableName qualifiableName = this.QualifiableName;
                TypeBindingResult typeBindingResult = bindingResolver.FindTypeBindingForType(qualifiableName);

                if (typeBindingResult is TypeBindingResult.Success successResult)
                {
                    this.typeBinding = successResult.TypeBinding;
                }
                else if (typeBindingResult is TypeBindingResult.Error errorResult)
                {
                    this.AddError(errorResult.Message);
                    this.typeBinding = new InvalidTypeBinding(qualifiableName);
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
                return this.typeBinding;
            }

            if (this.propertyBinding != null)
            {
                return this.propertyBinding.GetTypeBinding();
            }

            throw new Exception(
                $"Can't call GetTypeBinding on symbol reference that isn't a type; check IsType first");
        }

        public override bool IsTerminalNode() { return false; }

        public override SyntaxNodeType NodeType => SyntaxNodeType.SymbolReference;

        public override void VisitChildren(SyntaxNode.SyntaxVisitor visitor)
        {
            if (this.qualifier != null)
            {
                visitor(this.qualifier);
            }

            visitor(this.symbol);
        }

        public override void WriteSource(SourceWriter sourceWriter)
        {
            if (this.qualifier != null)
            {
                this.qualifier.WriteSource(sourceWriter);
                sourceWriter.Write(".");
            }

            sourceWriter.Write(this.symbol);
        }
    }
}
