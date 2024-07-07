using Faml.Api;
using Faml.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax
{
    /// <summary>
    /// A PropertySpecifier is a property name followed by a colon. It's typically followed by
    /// the property type or property value.
    /// </summary>
    public sealed class PropertySpecifierSyntax : SyntaxNode
    {
        private readonly QualifiableNameSyntax propertyName;

        public PropertySpecifierSyntax(TextSpan span, QualifiableNameSyntax propertyName) : base(span)
        {
            this.propertyName = propertyName;
            this.propertyName.SetParent(this);
        }

        public QualifiableNameSyntax PropertyNameSyntax => this.propertyName;

        public QualifiableName PropertyName => this.propertyName.Name;

        public override void VisitChildren(SyntaxVisitor visitor)
        {
            visitor(this.propertyName);
        }

        public override bool IsTerminalNode() => false;

        public override SyntaxNodeType NodeType => SyntaxNodeType.PropertySpecifier;

        public override void WriteSource(SourceWriter sourceWriter)
        {
            sourceWriter.Write(this.propertyName);
            sourceWriter.Write(":");
        }
    }
}
