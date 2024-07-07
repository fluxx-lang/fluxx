/**
 * @author Bret Johnson
 * @since 6/29/2014 2:15 AM
 */

using Faml.Api;
using Faml.Binding;
using Faml.Binding.Resolver;
using Faml.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax.Expression
{
    public sealed class PropertyAccessSyntax : ExpressionSyntax
    {
        private readonly ExpressionSyntax expression;
        private readonly NameSyntax propertyNameSyntax;
        private PropertyBinding propertyBinding = null;
        private TypeBinding typeBinding = null;

        public PropertyAccessSyntax(TextSpan span, ExpressionSyntax expression, NameSyntax propertyNameSyntax) : base(span)
        {
            this.expression = expression;
            expression.SetParent(this);

            this.propertyNameSyntax = propertyNameSyntax;
            propertyNameSyntax.SetParent(this);
        }

        public ExpressionSyntax Expression => this.expression;

        public NameSyntax PropertyNameSyntax => this.propertyNameSyntax;

        public Name PropertyName => this.propertyNameSyntax.Name;

        public string PropertyNameString => this.propertyNameSyntax.Name.ToString();

        public PropertyBinding PropertyBinding => this.propertyBinding;

        public override bool IsTerminalNode()
        {
            return false;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.PropertyAccess;

        public override void VisitChildren(SyntaxVisitor visitor)
        {
            visitor(this.expression);
            visitor(this.propertyNameSyntax);
        }

        public override void WriteSource(SourceWriter sourceWriter)
        {
            sourceWriter.Write(this.expression);
            sourceWriter.Write(".");
            sourceWriter.Write(this.propertyNameSyntax);
        }

        protected internal override void ResolveBindings(BindingResolver bindingResolver)
        {
            this.propertyBinding = bindingResolver.ResolvePropertyBinding(this.expression.GetTypeBinding(), this.propertyNameSyntax);
            this.typeBinding = this.propertyBinding.GetTypeBinding();
        }

        public override TypeBinding GetTypeBinding()
        {
            return this.typeBinding;
        }
    }
}
