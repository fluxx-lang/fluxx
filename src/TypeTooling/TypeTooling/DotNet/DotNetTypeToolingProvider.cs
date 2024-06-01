using TypeTooling.DotNet.RawTypes;
using TypeTooling.DotNet.Types;
using TypeTooling.RawTypes;
using TypeTooling.Types;
using TypeTooling.Types.PredefinedTypes;

namespace TypeTooling.DotNet {
    public class DotNetTypeToolingProvider : TypeToolingProvider {
        public DotNetTypeToolingProvider(TypeToolingEnvironment typeToolingEnvironment) : base(typeToolingEnvironment) {
        }

        public override TypeToolingType? ProvideType(RawType rawType, RawType? companionRawType) {
            if (!(rawType is DotNetRawType dotNetRawType))
                return null;

            var companionDotNetTypeDescriptor = (DotNetRawType?) companionRawType;
            string typeName = dotNetRawType.FullName;

            if (typeName == "System.Boolean")
                return BooleanType.Instance;
            else if (typeName == "System.String")
                return StringType.Instance;
            else if (typeName == "System.Int32")
                return IntegerType.Instance;
            else if (dotNetRawType.IsEnum)
                return new DotNetEnumType(dotNetRawType);

            DotNetRawType? enumerableElementType = dotNetRawType.GetEnumerableElementType();
            if (enumerableElementType != null)
                return new DotNetSequenceType(TypeToolingEnvironment, dotNetRawType, enumerableElementType);

            return new DotNetObjectType(TypeToolingEnvironment, dotNetRawType, companionDotNetTypeDescriptor);
        }

        public override AttachedType? ProvideAttachedType(RawType rawType, RawType? companionRawType) {
            // TODO: Implement this

            return null;
        }
    }
}
