using Faml.Binding;
using Faml.Binding.Resolver;
using Faml.CodeAnalysis.Text;
using Faml.Syntax.Expression;
using Faml.Syntax.Literal;
using Microsoft.CodeAnalysisP.Text;

namespace Faml.Syntax {
    public sealed class ContentArgumentSyntax : SyntaxNode {
        private ExpressionSyntax _value;
        private TypeBinding? _parameterTypeBinding;


        public ContentArgumentSyntax(TextSpan span, ExpressionSyntax value) : base(span) {
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

        public ExpressionSyntax Value => _value;

        public TypeBinding ParameterTypeBinding => _parameterTypeBinding;

        public override void VisitChildren(SyntaxVisitor visitor) {
            if (_value != null)
                visitor(_value);
        }

        public override bool IsTerminalNode() => false;

        public override SyntaxNodeType NodeType => SyntaxNodeType.ArgumentNameValuePair;

        public override void WriteSource(SourceWriter sourceWriter) {
            sourceWriter.Write(_value);
        }
    }
}
