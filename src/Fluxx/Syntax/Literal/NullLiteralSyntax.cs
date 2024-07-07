using Fluxx.Binding;
using Fluxx.CodeAnalysis.Text;
using Fluxx.Syntax.Expression;
using Microsoft.CodeAnalysis.Text;

namespace Fluxx.Syntax.Literal
{
    public class NullLiteralSyntax : ExpressionSyntax
    {
        public NullLiteralSyntax(TextSpan span) : base(span) {}

        public override Binding.TypeBinding GetTypeBinding()
        {
            return BuiltInTypeBinding.Null;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.NullLiteral;

        public override void WriteSource(SourceWriter sourceWriter)
        {
            sourceWriter.Write("null");
        }
    }
}
