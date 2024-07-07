
/**
 * Created by Bret on 6/28/2014.
 */

using Faml.Binding;
using Faml.CodeAnalysis.Text;
using Faml.Syntax.Expression;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax.Literal
{
    public class StringLiteralSyntax : ExpressionSyntax
    {
        private readonly string _value;

        public StringLiteralSyntax(TextSpan span, string value) : base(span)
        {
            this._value = value;
        }

        public override Binding.TypeBinding GetTypeBinding()
        {
            return BuiltInTypeBinding.String;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.StringLiteral;

        public override void WriteSource(SourceWriter sourceWriter)
        {
            sourceWriter.Write(this._value);
        }

        public virtual string Value => this._value;
    }
}
