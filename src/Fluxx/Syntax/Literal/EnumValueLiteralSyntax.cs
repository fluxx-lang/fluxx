using Faml.Api;
using Faml.Binding;
using Faml.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax.Expression {
    public class EnumValueLiteralSyntax : ExpressionSyntax {
        private readonly NameSyntax _enumValueSyntax;
        private readonly EnumValueBinding _enumValueBinding;


        public EnumValueLiteralSyntax(TextSpan span, NameSyntax enumValueSyntax, EnumValueBinding enumValueBinding) : base(span) {
            this._enumValueSyntax = enumValueSyntax;
            this._enumValueSyntax.SetParent(this);

            this._enumValueBinding = enumValueBinding;
        }

        public NameSyntax EnumValueSyntax => this._enumValueSyntax;

        public Name EnumValueName => this._enumValueSyntax.Name;

        public EnumValueBinding EnumValueBinding => this._enumValueBinding;

        public override TypeBinding GetTypeBinding() {
            return this._enumValueBinding.EnumTypeBinding;
        }

        public override bool IsTerminalNode() {
            return false;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.EnumValueLiteral;

        public override void VisitChildren(SyntaxVisitor visitor) {
            if (this._enumValueSyntax != null)
            {
                visitor(this._enumValueSyntax);
            }
        }

        public override void WriteSource(SourceWriter sourceWriter) {
            sourceWriter.Write(this._enumValueSyntax.ToString());
        }
    }
}
