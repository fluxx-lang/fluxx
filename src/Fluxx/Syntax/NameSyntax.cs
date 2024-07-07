/**
 * @author Bret Johnson
 * @since 6/6/2015
 */

using Faml.Api;
using Faml.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax
{
    public class NameSyntax : SyntaxNode
    {
        private readonly Name name;

        public NameSyntax(TextSpan span, Name name) : base(span)
        {
            this.name = name;
        }

        public Name Name => this.name;

        public override SyntaxNodeType NodeType => SyntaxNodeType.NameIdentifier;

        public override void WriteSource(SourceWriter sourceWriter)
        {
            sourceWriter.Write(this.name);
        }
    }
}
