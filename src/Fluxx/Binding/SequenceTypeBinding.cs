using Faml.Api;
using Faml.Syntax;
using Faml.Syntax.Expression;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Binding
{
    public class SequenceTypeBinding : ObjectTypeBinding
    {
        private readonly TypeBinding _elementType;


        public SequenceTypeBinding(TypeBinding elementType) : base(new QualifiableName("List<" + elementType.TypeName+ ">"))
        {
            this._elementType = elementType;
        }

        public TypeBinding ElementType => this._elementType;

        public override bool SupportsCreateLiteral() => false;

        public override ExpressionSyntax ParseLiteralValueSource(FamlModule module, TextSpan sourceText)
        {
            throw new System.NotImplementedException();
        }
    }
}
