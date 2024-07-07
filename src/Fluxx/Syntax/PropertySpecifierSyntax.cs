using Faml.Api;
using Faml.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax {
    /// <summary>
    /// A PropertySpecifier is a property name followed by a colon. It's typically followed by
    /// the property type or property value.
    /// </summary>
    public sealed class PropertySpecifierSyntax : SyntaxNode {
        private readonly QualifiableNameSyntax _propertyName;

        public PropertySpecifierSyntax(TextSpan span, QualifiableNameSyntax propertyName) : base(span) {
            _propertyName = propertyName;
            _propertyName.SetParent(this);
        }


        public QualifiableNameSyntax PropertyNameSyntax => _propertyName;

        public QualifiableName PropertyName => _propertyName.Name;

        public override void VisitChildren(SyntaxVisitor visitor) {
            visitor(_propertyName);
        }

        public override bool IsTerminalNode() => false;

        public override SyntaxNodeType NodeType => SyntaxNodeType.PropertySpecifier;

        public override void WriteSource(SourceWriter sourceWriter) {
            sourceWriter.Write(_propertyName);
            sourceWriter.Write(":");
        }
    }
}
