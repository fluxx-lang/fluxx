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
            this._expression = expression;
            expression.SetParent(this);

            this._propertyNameSyntax = propertyNameSyntax;
            propertyNameSyntax.SetParent(this);
        }

        public ExpressionSyntax Expression => this._expression;

        public NameSyntax PropertyNameSyntax => this._propertyNameSyntax;

        public Name PropertyName => this._propertyNameSyntax.Name;

        public string PropertyNameString => this._propertyNameSyntax.Name.ToString();

        public PropertyBinding PropertyBinding => this._propertyBinding;

        public override bool IsTerminalNode() {
            return false;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.PropertyAccess;

        public override void VisitChildren(SyntaxVisitor visitor) {
            visitor(this._expression);
            visitor(this._propertyNameSyntax);
        }

        public override void WriteSource(SourceWriter sourceWriter) {
            sourceWriter.Write(this._expression);
            sourceWriter.Write(".");
            sourceWriter.Write(this._propertyNameSyntax);
        }

        protected internal override void ResolveBindings(BindingResolver bindingResolver) {
            this._propertyBinding = bindingResolver.ResolvePropertyBinding(this._expression.GetTypeBinding(), this._propertyNameSyntax);
            this._typeBinding = this._propertyBinding.GetTypeBinding();
        }

        public override TypeBinding GetTypeBinding() {
            return this._typeBinding;
        }
    }
}
