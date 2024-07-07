using Faml.Binding;
using Faml.Binding.Resolver;
using Faml.CodeAnalysis.Text;
using Faml.Syntax.Expression;
using Faml.Syntax.Literal;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax {
    public sealed class ContentArgumentSyntax : SyntaxNode {
        private ExpressionSyntax _value;
        private TypeBinding? _parameterTypeBinding;


        public ContentArgumentSyntax(TextSpan span, ExpressionSyntax value) : base(span) {
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

        public ExpressionSyntax Value => this._value;

        public TypeBinding ParameterTypeBinding => this._parameterTypeBinding;

        public override void VisitChildren(SyntaxVisitor visitor) {
            if (this._value != null)
                visitor(this._value);
        }

        public override bool IsTerminalNode() => false;

        public override SyntaxNodeType NodeType => SyntaxNodeType.ArgumentNameValuePair;

        public override void WriteSource(SourceWriter sourceWriter) {
            sourceWriter.Write(this._value);
        }
    }
}
