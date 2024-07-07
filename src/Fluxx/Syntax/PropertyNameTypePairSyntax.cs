using Faml.Api;
using Faml.CodeAnalysis.Text;
using Faml.Syntax.Expression;
using Faml.Syntax.Type;
using Microsoft.CodeAnalysis.Text;

/**
 * Created by Bret on 6/28/2014.
 */
namespace Faml.Syntax
{
    public sealed class PropertyNameTypePairSyntax : SyntaxNode
    {
        private readonly NameSyntax propertyNameSyntax;
        private readonly TypeReferenceSyntax typeReferenceSyntax;
        private readonly ExpressionSyntax? defaultValue;
        // TODO: Handle inferred types here

        // AST structure properties
        public PropertyNameTypePairSyntax(TextSpan span, NameSyntax propertyNameSyntax, TypeReferenceSyntax typeReferenceSyntax, ExpressionSyntax? defaultValue) : base(span)
            {
            this.propertyNameSyntax = propertyNameSyntax;
            this.propertyNameSyntax.SetParent(this);

            this.typeReferenceSyntax = typeReferenceSyntax;
            this.typeReferenceSyntax.SetParent(this);

            this.defaultValue = defaultValue;
            this.defaultValue?.SetParent(this);
        }

        public NameSyntax PropertyNameSyntax => this.propertyNameSyntax;

        public Name PropertyName => this.propertyNameSyntax.Name;

        public TypeReferenceSyntax TypeReferenceSyntax => this.typeReferenceSyntax;

        public override void VisitChildren(SyntaxVisitor visitor)
        {
            visitor(this.propertyNameSyntax);
            visitor(this.typeReferenceSyntax);
            if (this.defaultValue != null)
            {
                visitor(this.defaultValue);
            }
        }

        public override bool IsTerminalNode() { return false; }
        public override SyntaxNodeType NodeType => SyntaxNodeType.PropertyNameTypePair;

        public override void WriteSource(SourceWriter sourceWriter)
        {
            sourceWriter.Write(this.propertyNameSyntax);
            sourceWriter.Write(":");
            sourceWriter.Write(this.typeReferenceSyntax);
            if (this.defaultValue != null)
            {
                sourceWriter.Write(" = ");
                sourceWriter.Write(this.defaultValue);
            }
        }
    }
}
