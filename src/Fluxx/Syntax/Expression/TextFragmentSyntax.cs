/**
 * Created by Bret on 6/28/2014.
 */

using Faml.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax.Expression {
    public sealed class TextFragmentSyntax : SyntaxNode {
        private readonly string _value;

        public TextFragmentSyntax(TextSpan span, string value) : base(span) { this._value = value; }

        public string Value => this._value;

        public override SyntaxNodeType NodeType => SyntaxNodeType.InterpolatedStringFragment;

        public override void WriteSource(SourceWriter sourceWriter) { sourceWriter.Write(this._value); }
    }
}
