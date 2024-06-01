using System.Collections.Immutable;
using TypeTooling.RawTypes;

namespace TypeTooling.CodeGeneration.Expressions
{
    public class NewSequenceCode : ExpressionCode {
        public RawType ElementType { get; }
        public ImmutableArray<ExpressionCode> Items { get; }

        public NewSequenceCode(RawType elementType, ImmutableArray<ExpressionCode> items) {
            ElementType = elementType;
            Items = items;
        }
    }
}
