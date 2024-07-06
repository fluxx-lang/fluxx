using TypeTooling.RawTypes;
using TypeTooling.Types;

namespace TypeTooling
{
    public abstract class TypeToolingProvider(TypeToolingEnvironment typeToolingEnvironment)
    {
        public TypeToolingEnvironment TypeToolingEnvironment { get; } = typeToolingEnvironment;

        public abstract TypeToolingType? ProvideType(RawType rawType, RawType? companionRawType);

        public abstract AttachedType? ProvideAttachedType(RawType rawType, RawType? companionRawType);
    }
}
