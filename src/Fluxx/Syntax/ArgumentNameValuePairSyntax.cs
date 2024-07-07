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
            _propertySpecifier = propertySpecifier;
            _propertySpecifier.SetParent(this);

            _value = value;
            value.SetParent(this);
        }

        public void ResolveValueBindings(TypeBinding parameterTypeBinding, BindingResolver bindingResolver) {
            // If we've already resolved, do nothing
            if (_parameterTypeBinding != null)
                return;
            
            _parameterTypeBinding = parameterTypeBinding;

            if (_value is TextualLiteralSyntax markupValue)
                _value = markupValue.ResolveMarkup(parameterTypeBinding, bindingResolver);
        }

        public PropertySpecifierSyntax PropertySpecifier => _propertySpecifier;

        public QualifiableName ArgumentName => _propertySpecifier.PropertyName;

        public ExpressionSyntax Value => _value;

        public TypeBinding ParameterTypeBinding => _parameterTypeBinding;

        public override void VisitChildren(SyntaxVisitor visitor) {
            visitor(_propertySpecifier);

            if (_value != null)
                visitor(_value);
        }

        public override bool IsTerminalNode() => false;

        public override SyntaxNodeType NodeType => SyntaxNodeType.ArgumentNameValuePair;

        public override void WriteSource(SourceWriter sourceWriter) {
            sourceWriter.Write(_propertySpecifier);
            sourceWriter.Write(_value);
        }
    }
}
