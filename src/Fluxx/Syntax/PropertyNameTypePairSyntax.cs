using Faml.Api;
using Faml.CodeAnalysis.Text;
using Faml.Syntax.Expression;
using Faml.Syntax.Type;
using Microsoft.CodeAnalysis.Text;

/**
 * Created by Bret on 6/28/2014.
 */
namespace Faml.Syntax {
    public sealed class PropertyNameTypePairSyntax : SyntaxNode {
        private readonly NameSyntax _propertyNameSyntax;
        private readonly TypeReferenceSyntax _typeReferenceSyntax;
        private readonly ExpressionSyntax? _defaultValue;
        // TODO: Handle inferred types here

        // AST structure properties
        public PropertyNameTypePairSyntax(TextSpan span, NameSyntax propertyNameSyntax, TypeReferenceSyntax typeReferenceSyntax, ExpressionSyntax? defaultValue) :
            base(span) {
            this._propertyNameSyntax = propertyNameSyntax;
            this._propertyNameSyntax.SetParent(this);

            this._typeReferenceSyntax = typeReferenceSyntax;
            this._typeReferenceSyntax.SetParent(this);

            this._defaultValue = defaultValue;
            this._defaultValue?.SetParent(this);
        }

        public NameSyntax PropertyNameSyntax => this._propertyNameSyntax;

        public Name PropertyName => this._propertyNameSyntax.Name;

        public TypeReferenceSyntax TypeReferenceSyntax => this._typeReferenceSyntax;

        public override void VisitChildren(SyntaxVisitor visitor) {
            visitor(this._propertyNameSyntax);
            visitor(this._typeReferenceSyntax);
            if (this._defaultValue != null)
                visitor(this._defaultValue);
        }

        public override bool IsTerminalNode() { return false; }
        public override SyntaxNodeType NodeType => SyntaxNodeType.PropertyNameTypePair;

        public override void WriteSource(SourceWriter sourceWriter) {
            sourceWriter.Write(this._propertyNameSyntax);
            sourceWriter.Write(":");
            sourceWriter.Write(this._typeReferenceSyntax);
            if (this._defaultValue != null) {
                sourceWriter.Write(" = ");
                sourceWriter.Write(this._defaultValue);
            }
        }
    }
}
