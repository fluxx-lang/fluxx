using System.Globalization;
using TypeTooling.RawTypes;
using TypeTooling.Types;

namespace TypeTooling {
    public abstract class TypeToolingEnvironment {
        public abstract TypeToolingType? GetType(RawType rawType);

        public abstract TypeToolingType GetRequiredType(RawType rawType);

        public abstract RawType? GetRawType(string rawTypeName);

        public abstract RawType GetRequiredRawType(string rawTypeName);

        public abstract object Instantiate(RawType rawType, params object[] args);

        public abstract CultureInfo UICulture { get; }
    }
}
