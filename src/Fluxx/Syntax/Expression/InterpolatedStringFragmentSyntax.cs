/**
 * Created by Bret on 6/28/2014.
 */

using Microsoft.CodeAnalysisP.Text;
using Faml.CodeAnalysis.Text;

namespace Faml.Syntax.Expression {
    public sealed class InterpolatedStringFragmentSyntax : SyntaxNode {
        private readonly string _value;

        public InterpolatedStringFragmentSyntax(TextSpan span, string value) : base(span) { this._value = value; }

        public string Value => _value;

        public override SyntaxNodeType NodeType => SyntaxNodeType.InterpolatedStringFragment;

        public override void WriteSource(SourceWriter sourceWriter) { sourceWriter.Write(_value); }
    }
}
