using Faml.Api;
using Microsoft.CodeAnalysisP.Text;
using Faml.CodeAnalysis.Text;

/**
 * @author Bret Johnson
 * @since 6/6/2015
 */

namespace Faml.Syntax {
    public class QualifiableNameSyntax : SyntaxNode {
        private readonly QualifiableName _name;

        public QualifiableNameSyntax(TextSpan span, QualifiableName name) : base(span) {
            _name = name;
        }

        public QualifiableName Name => _name;

        public override SyntaxNodeType NodeType => SyntaxNodeType.QualifiableName;

        public override void WriteSource(SourceWriter sourceWriter) {
            sourceWriter.Write(_name);
        }
    }
}
