using Fluxx.Binding;
using Fluxx.CodeAnalysis.Text;
using Fluxx.Syntax.Expression;
using Microsoft.CodeAnalysis.Text;

namespace Fluxx.Syntax.Literal
{
    public class StringLiteralSyntax : ExpressionSyntax
    {
        private readonly string value;

        public StringLiteralSyntax(TextSpan span, string value) : base(span)
        {
            this.value = value;
        }

        public override Binding.TypeBinding GetTypeBinding()
        {
            return BuiltInTypeBinding.String;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.StringLiteral;

        public override void WriteSource(SourceWriter sourceWriter)
        {
            sourceWriter.Write(this.value);
        }

        public virtual string Value => this.value;
    }
}
