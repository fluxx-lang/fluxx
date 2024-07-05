using System.Collections.Immutable;
using TypeTooling.RawTypes;

namespace TypeTooling.CodeGeneration.Expressions
{
    public class NewArrayInitCode : ExpressionCode
    {
        public RawType ElementType { get; }
        public ImmutableArray<ExpressionCode> Items { get; }

        public NewArrayInitCode(RawType elementType, ImmutableArray<ExpressionCode> items)
        {
            this.ElementType = elementType;
            this.Items = items;
        }
    }
}
