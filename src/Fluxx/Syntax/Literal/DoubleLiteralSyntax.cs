using Faml.Binding;
using Faml.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

/**
 * Created by Bret on 6/28/2014.
 */
namespace Faml.Syntax.Literal {
    public sealed class DoubleLiteralSyntax : Expression.ExpressionSyntax {
        private readonly int _value;

        public DoubleLiteralSyntax(TextSpan span, int value) : base(span) {
            _value = value;
        }

        public override TypeBinding GetTypeBinding() {
            return BuiltInTypeBinding.Double;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.DoubleLiteral;

        public override void WriteSource(SourceWriter sourceWriter) {
            sourceWriter.Write(_value.ToString());
        }

        public double Value => _value;
    }
}
