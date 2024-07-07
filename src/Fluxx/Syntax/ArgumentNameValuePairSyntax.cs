using Fluxx.Api;
using Fluxx.Binding;
using Fluxx.Binding.Resolver;
using Fluxx.CodeAnalysis.Text;
using Fluxx.Syntax.Expression;
using Fluxx.Syntax.Literal;
using Microsoft.CodeAnalysis.Text;

namespace Fluxx.Syntax
{
    public sealed class ArgumentNameValuePairSyntax : SyntaxNode
    {
        private readonly PropertySpecifierSyntax propertySpecifier;
        private ExpressionSyntax value;
        private TypeBinding? parameterTypeBinding;

        public ArgumentNameValuePairSyntax(TextSpan span, PropertySpecifierSyntax propertySpecifier, ExpressionSyntax value) : base(span)
        {
            this.propertySpecifier = propertySpecifier;
            this.propertySpecifier.SetParent(this);

            this.value = value;
            value.SetParent(this);
        }

        public void ResolveValueBindings(TypeBinding parameterTypeBinding, BindingResolver bindingResolver)
        {
            // If we've already resolved, do nothing
            if (this.parameterTypeBinding != null)
            {
                return;
            }

            this.parameterTypeBinding = parameterTypeBinding;

            if (this.value is TextualLiteralSyntax markupValue)
            {
                this.value = markupValue.ResolveMarkup(parameterTypeBinding, bindingResolver);
            }
        }

        public PropertySpecifierSyntax PropertySpecifier => this.propertySpecifier;

        public QualifiableName ArgumentName => this.propertySpecifier.PropertyName;

        public ExpressionSyntax Value => this.value;

        public TypeBinding ParameterTypeBinding => this.parameterTypeBinding;

        public override void VisitChildren(SyntaxVisitor visitor)
        {
            visitor(this.propertySpecifier);

            if (this.value != null)
            {
                visitor(this.value);
            }
        }

        public override bool IsTerminalNode() => false;

        public override SyntaxNodeType NodeType => SyntaxNodeType.ArgumentNameValuePair;

        public override void WriteSource(SourceWriter sourceWriter)
        {
            sourceWriter.Write(this.propertySpecifier);
            sourceWriter.Write(this.value);
        }
    }
}
