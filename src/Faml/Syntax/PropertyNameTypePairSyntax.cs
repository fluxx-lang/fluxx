using Faml.Api;
using Microsoft.CodeAnalysisP.Text;
using Faml.CodeAnalysis.Text;
using Faml.Syntax.Type;
using Faml.Syntax.Expression;

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
            _propertyNameSyntax = propertyNameSyntax;
            _propertyNameSyntax.SetParent(this);

            _typeReferenceSyntax = typeReferenceSyntax;
            _typeReferenceSyntax.SetParent(this);

            _defaultValue = defaultValue;
            _defaultValue?.SetParent(this);
        }

        public NameSyntax PropertyNameSyntax => _propertyNameSyntax;

        public Name PropertyName => _propertyNameSyntax.Name;

        public TypeReferenceSyntax TypeReferenceSyntax => _typeReferenceSyntax;

        public override void VisitChildren(SyntaxVisitor visitor) {
            visitor(_propertyNameSyntax);
            visitor(_typeReferenceSyntax);
            if (_defaultValue != null)
                visitor(_defaultValue);
        }

        public override bool IsTerminalNode() { return false; }
        public override SyntaxNodeType NodeType => SyntaxNodeType.PropertyNameTypePair;

        public override void WriteSource(SourceWriter sourceWriter) {
            sourceWriter.Write(_propertyNameSyntax);
            sourceWriter.Write(":");
            sourceWriter.Write(_typeReferenceSyntax);
            if (_defaultValue != null) {
                sourceWriter.Write(" = ");
                sourceWriter.Write(_defaultValue);
            }
        }
    }
}
