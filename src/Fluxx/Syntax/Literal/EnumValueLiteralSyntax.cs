using Faml.Api;
using Faml.Binding;
using Faml.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax.Expression
{
    public class EnumValueLiteralSyntax : ExpressionSyntax
    {
        private readonly NameSyntax enumValueSyntax;
        private readonly EnumValueBinding enumValueBinding;


        public EnumValueLiteralSyntax(TextSpan span, NameSyntax enumValueSyntax, EnumValueBinding enumValueBinding) : base(span)
        {
            this.enumValueSyntax = enumValueSyntax;
            this.enumValueSyntax.SetParent(this);

            this.enumValueBinding = enumValueBinding;
        }

        public NameSyntax EnumValueSyntax => this.enumValueSyntax;

        public Name EnumValueName => this.enumValueSyntax.Name;

        public EnumValueBinding EnumValueBinding => this.enumValueBinding;

        public override TypeBinding GetTypeBinding()
        {
            return this.enumValueBinding.EnumTypeBinding;
        }

        public override bool IsTerminalNode()
        {
            return false;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.EnumValueLiteral;

        public override void VisitChildren(SyntaxVisitor visitor)
        {
            if (this.enumValueSyntax != null)
            {
                visitor(this.enumValueSyntax);
            }
        }

        public override void WriteSource(SourceWriter sourceWriter)
        {
            sourceWriter.Write(this.enumValueSyntax.ToString());
        }
    }
}
