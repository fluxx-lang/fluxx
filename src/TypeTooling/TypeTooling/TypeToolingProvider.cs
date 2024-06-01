using TypeTooling.RawTypes;
using TypeTooling.Types;

namespace TypeTooling {
    public abstract class TypeToolingProvider {
        public TypeToolingEnvironment TypeToolingEnvironment { get;  }

        protected TypeToolingProvider(TypeToolingEnvironment typeToolingEnvironment) {
            TypeToolingEnvironment = typeToolingEnvironment;
        }

        public abstract TypeToolingType? ProvideType(RawType rawType, RawType? companionRawType);

        public abstract AttachedType? ProvideAttachedType(RawType rawType, RawType? companionRawType);
    }
}
