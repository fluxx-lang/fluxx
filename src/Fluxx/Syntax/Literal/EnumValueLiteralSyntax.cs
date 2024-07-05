using Faml.Api;
using Microsoft.CodeAnalysisP.Text;
using Faml.Binding;
using Faml.CodeAnalysis.Text;

namespace Faml.Syntax.Expression {
    public class EnumValueLiteralSyntax : ExpressionSyntax {
        private readonly NameSyntax _enumValueSyntax;
        private readonly EnumValueBinding _enumValueBinding;


        public EnumValueLiteralSyntax(TextSpan span, NameSyntax enumValueSyntax, EnumValueBinding enumValueBinding) : base(span) {
            _enumValueSyntax = enumValueSyntax;
            _enumValueSyntax.SetParent(this);

            _enumValueBinding = enumValueBinding;
        }

        public NameSyntax EnumValueSyntax => _enumValueSyntax;

        public Name EnumValueName => _enumValueSyntax.Name;

        public EnumValueBinding EnumValueBinding => _enumValueBinding;

        public override TypeBinding GetTypeBinding() {
            return _enumValueBinding.EnumTypeBinding;
        }

        public override bool IsTerminalNode() {
            return false;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.EnumValueLiteral;

        public override void VisitChildren(SyntaxVisitor visitor) {
            if (_enumValueSyntax != null)
                visitor(_enumValueSyntax);
        }

        public override void WriteSource(SourceWriter sourceWriter) {
            sourceWriter.Write(_enumValueSyntax.ToString());
        }
    }
}
