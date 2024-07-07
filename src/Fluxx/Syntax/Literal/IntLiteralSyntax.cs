using Faml.Binding;
using Faml.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax.Literal {
    public sealed class IntLiteralSyntax : Expression.ExpressionSyntax {
        private readonly int _value;

        public IntLiteralSyntax(TextSpan span, int value) : base(span) {
            _value = value;
        }

        public override TypeBinding GetTypeBinding() {
            return BuiltInTypeBinding.Int;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.IntLiteral;

        public override void WriteSource(SourceWriter sourceWriter) {
            sourceWriter.Write(_value.ToString());
        }

        public int Value => _value;
    }
}
