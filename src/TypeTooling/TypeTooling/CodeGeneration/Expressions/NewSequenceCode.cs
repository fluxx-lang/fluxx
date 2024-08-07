﻿using System.Collections.Immutable;
using TypeTooling.RawTypes;

namespace TypeTooling.CodeGeneration.Expressions
{
    public class NewSequenceCode(RawType elementType, ImmutableArray<ExpressionCode> items) : ExpressionCode
    {
        public RawType ElementType { get; } = elementType;

        public ImmutableArray<ExpressionCode> Items { get; } = items;
    }
}
