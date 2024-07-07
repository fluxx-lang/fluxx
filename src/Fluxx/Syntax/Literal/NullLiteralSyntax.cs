using Faml.Binding;
using Faml.CodeAnalysis.Text;
using Faml.Syntax.Expression;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax.Literal
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
