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
        private readonly QualifiableName name;

        public QualifiableNameSyntax(TextSpan span, QualifiableName name) : base(span)
        {
            this.name = name;
        }

        public QualifiableName Name => this.name;

        public override SyntaxNodeType NodeType => SyntaxNodeType.QualifiableName;

        public override void WriteSource(SourceWriter sourceWriter)
        {
            sourceWriter.Write(this.name);
        }
    }
}
