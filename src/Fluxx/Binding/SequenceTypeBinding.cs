using Fluxx.Api;
using Fluxx.Syntax;
using Fluxx.Syntax.Expression;
using Microsoft.CodeAnalysis.Text;

namespace Fluxx.Binding
{
    public class SequenceTypeBinding : ObjectTypeBinding
    {
        private readonly TypeBinding elementType;

        public SequenceTypeBinding(TypeBinding elementType) : base(new QualifiableName("List<" + elementType.TypeName + ">"))
        {
            this.elementType = elementType;
        }

        public TypeBinding ElementType => this.elementType;

        public override bool SupportsCreateLiteral() => false;

        public override ExpressionSyntax ParseLiteralValueSource(FamlModule module, TextSpan sourceText)
        {
            throw new System.NotImplementedException();
        }
    }
}
