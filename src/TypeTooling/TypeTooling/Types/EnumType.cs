using System.Collections.Generic;

namespace TypeTooling.Types
{
    public abstract class EnumType : TypeToolingType
    {
        public abstract IReadOnlyCollection<EnumValue> Values { get; }
    }
}
