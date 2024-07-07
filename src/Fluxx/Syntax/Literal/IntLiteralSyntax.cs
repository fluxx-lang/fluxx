using Fluxx.Binding;
using Fluxx.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

namespace Fluxx.Syntax.Literal
{
    public sealed class IntLiteralSyntax : Expression.ExpressionSyntax
    {
        private readonly int value;

        public IntLiteralSyntax(TextSpan span, int value) : base(span)
        {
            this.value = value;
        }

        public override TypeBinding GetTypeBinding()
        {
            return BuiltInTypeBinding.Int;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.IntLiteral;

        public override void WriteSource(SourceWriter sourceWriter)
        {
            sourceWriter.Write(this.value.ToString());
        }

        public int Value => this.value;
    }
}
