/**
 * @author Bret Johnson
 * @since 4/15/2015
 */

using Faml.Api;
using Microsoft.CodeAnalysisP.Text;
using Faml.Syntax;
using Faml.Syntax.Expression;

namespace Faml.Binding {
    public class SequenceTypeBinding : ObjectTypeBinding {
        private readonly TypeBinding _elementType;


        public SequenceTypeBinding(TypeBinding elementType) : base(new QualifiableName("List<" + elementType.TypeName+ ">")) {
            _elementType = elementType;
        }

        public TypeBinding ElementType => _elementType;

        public override bool SupportsCreateLiteral() => false;

        public override ExpressionSyntax ParseLiteralValueSource(FamlModule module, TextSpan sourceText) {
            throw new System.NotImplementedException();
        }
    }
}
