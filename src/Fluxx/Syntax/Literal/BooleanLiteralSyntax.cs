/**
 * Created by Bret on 6/28/2014.
 */

using Faml.Binding;
using Faml.CodeAnalysis.Text;
using Faml.Syntax.Expression;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax.Literal
{
    public class BooleanLiteralSyntax : ExpressionSyntax
    {
        private readonly bool value;

        public  BooleanLiteralSyntax(TextSpan span, bool value) : base(span)
        {
            this.value = value;
        }

        public override TypeBinding GetTypeBinding()
        {
            return BuiltInTypeBinding.Bool;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.BooleanLiteral;

        public override void WriteSource(SourceWriter sourceWriter)
        {
            sourceWriter.Write(this.value ? "true" : "false");
        }

        public virtual bool Value => this.value;
    }
}
