using Faml.Api;
using Faml.Binding;
using Faml.Binding.Resolver;
using Faml.CodeAnalysis.Text;
using Faml.Syntax.Expression;
using Faml.Syntax.Literal;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax {
    public sealed class ArgumentNameValuePairSyntax : SyntaxNode {
        private readonly PropertySpecifierSyntax _propertySpecifier;
        private ExpressionSyntax _value;
        private TypeBinding? _parameterTypeBinding;


        public ArgumentNameValuePairSyntax(TextSpan span, PropertySpecifierSyntax propertySpecifier, ExpressionSyntax value) : base(span) {
            this._propertySpecifier = propertySpecifier;
            this._propertySpecifier.SetParent(this);

            this._value = value;
            value.SetParent(this);
        }

        public void ResolveValueBindings(TypeBinding parameterTypeBinding, BindingResolver bindingResolver) {
            // If we've already resolved, do nothing
            if (this._parameterTypeBinding != null)
                return;
            
            this._parameterTypeBinding = parameterTypeBinding;

            if (this._value is TextualLiteralSyntax markupValue)
                this._value = markupValue.ResolveMarkup(parameterTypeBinding, bindingResolver);
        }

        public PropertySpecifierSyntax PropertySpecifier => this._propertySpecifier;

        public QualifiableName ArgumentName => this._propertySpecifier.PropertyName;

        public ExpressionSyntax Value => this._value;

        public TypeBinding ParameterTypeBinding => this._parameterTypeBinding;

        public override void VisitChildren(SyntaxVisitor visitor) {
            visitor(this._propertySpecifier);

            if (this._value != null)
                visitor(this._value);
        }

        public override bool IsTerminalNode() => false;

        public override SyntaxNodeType NodeType => SyntaxNodeType.ArgumentNameValuePair;

        public override void WriteSource(SourceWriter sourceWriter) {
            sourceWriter.Write(this._propertySpecifier);
            sourceWriter.Write(this._value);
        }
    }
}
