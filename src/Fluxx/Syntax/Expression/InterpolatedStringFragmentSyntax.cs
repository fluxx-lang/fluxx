using Fluxx.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

namespace Fluxx.Syntax.Expression
{
    public sealed class InterpolatedStringFragmentSyntax : SyntaxNode
    {
        private readonly string value;

        public InterpolatedStringFragmentSyntax(TextSpan span, string value) : base(span) { this.value = value; }

        public string Value => this.value;

        public override SyntaxNodeType NodeType => SyntaxNodeType.InterpolatedStringFragment;

        public override void WriteSource(SourceWriter sourceWriter) { sourceWriter.Write(this.value); }
    }
}
