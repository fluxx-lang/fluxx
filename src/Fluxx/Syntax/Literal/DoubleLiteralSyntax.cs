using Fluxx.Binding;
using Fluxx.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

namespace Fluxx.Syntax.Literal
{
    public sealed class DoubleLiteralSyntax : Expression.ExpressionSyntax
    {
        private readonly int value;

        public DoubleLiteralSyntax(TextSpan span, int value) : base(span)
        {
            this.value = value;
        }

        public override TypeBinding GetTypeBinding()
        {
            return BuiltInTypeBinding.Double;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.DoubleLiteral;

        public override void WriteSource(SourceWriter sourceWriter)
        {
            sourceWriter.Write(this.value.ToString());
        }

        public double Value => this.value;
    }
}
