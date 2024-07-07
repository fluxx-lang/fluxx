/**
 * @author Bret Johnson
 * @since 6/29/2014 2:15 AM
 */

using Faml.Api;
using Faml.Binding;
using Faml.Binding.Resolver;
using Faml.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax.Expression {
    public sealed class PropertyAccessSyntax : ExpressionSyntax {
        private readonly ExpressionSyntax _expression;
        private readonly NameSyntax _propertyNameSyntax;
        private PropertyBinding _propertyBinding = null;
        private TypeBinding _typeBinding = null;

        public PropertyAccessSyntax(TextSpan span, ExpressionSyntax expression, NameSyntax propertyNameSyntax) : base(span) {
            _expression = expression;
            expression.SetParent(this);

            _propertyNameSyntax = propertyNameSyntax;
            propertyNameSyntax.SetParent(this);
        }

        public ExpressionSyntax Expression => _expression;

        public NameSyntax PropertyNameSyntax => _propertyNameSyntax;

        public Name PropertyName => _propertyNameSyntax.Name;

        public string PropertyNameString => _propertyNameSyntax.Name.ToString();

        public PropertyBinding PropertyBinding => _propertyBinding;

        public override bool IsTerminalNode() {
            return false;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.PropertyAccess;

        public override void VisitChildren(SyntaxVisitor visitor) {
            visitor(_expression);
            visitor(_propertyNameSyntax);
        }

        public override void WriteSource(SourceWriter sourceWriter) {
            sourceWriter.Write(_expression);
            sourceWriter.Write(".");
            sourceWriter.Write(_propertyNameSyntax);
        }

        protected internal override void ResolveBindings(BindingResolver bindingResolver) {
            _propertyBinding = bindingResolver.ResolvePropertyBinding(_expression.GetTypeBinding(), _propertyNameSyntax);
            _typeBinding = _propertyBinding.GetTypeBinding();
        }

        public override TypeBinding GetTypeBinding() {
            return _typeBinding;
        }
    }
}
