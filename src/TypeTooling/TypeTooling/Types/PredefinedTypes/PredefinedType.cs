using System;
using TypeTooling.RawTypes;

namespace TypeTooling.Types.PredefinedTypes
{
    public abstract class PredefinedType : TypeToolingType
    {
        public override RawType UnderlyingType => throw new InvalidOperationException($"Predefined types, like this {this.GetType().FullName} type, don't have an UnderlyingType since they can target any runtime system. Use e.g. ReflectionDotNetRawType.ForPredefinedType instead, for the desired runtime system.");
    }
}
