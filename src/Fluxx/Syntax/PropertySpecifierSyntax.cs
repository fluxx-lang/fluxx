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
        private readonly QualifiableNameSyntax _propertyName;

        public PropertySpecifierSyntax(TextSpan span, QualifiableNameSyntax propertyName) : base(span)
        {
            this._propertyName = propertyName;
            this._propertyName.SetParent(this);
        }


        public QualifiableNameSyntax PropertyNameSyntax => this._propertyName;

        public QualifiableName PropertyName => this._propertyName.Name;

        public override void VisitChildren(SyntaxVisitor visitor)
        {
            visitor(this._propertyName);
        }

        public override bool IsTerminalNode() => false;

        public override SyntaxNodeType NodeType => SyntaxNodeType.PropertySpecifier;

        public override void WriteSource(SourceWriter sourceWriter)
        {
            sourceWriter.Write(this._propertyName);
            sourceWriter.Write(":");
        }
    }
}
