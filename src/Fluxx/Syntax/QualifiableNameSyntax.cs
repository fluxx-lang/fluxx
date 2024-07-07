using Faml.Api;
using Faml.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

/**
 * @author Bret Johnson
 * @since 6/6/2015
 */

namespace Faml.Syntax
{
    public class QualifiableNameSyntax : SyntaxNode
    {
        private readonly QualifiableName _name;

        public QualifiableNameSyntax(TextSpan span, QualifiableName name) : base(span)
        {
            this._name = name;
        }

        public QualifiableName Name => this._name;

        public override SyntaxNodeType NodeType => SyntaxNodeType.QualifiableName;

        public override void WriteSource(SourceWriter sourceWriter)
        {
            sourceWriter.Write(this._name);
        }
    }
}
