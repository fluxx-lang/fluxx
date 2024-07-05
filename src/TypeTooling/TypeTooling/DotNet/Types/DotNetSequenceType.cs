using TypeTooling.DotNet.RawTypes;
using TypeTooling.Helper;
using TypeTooling.RawTypes;
using TypeTooling.Types;

namespace TypeTooling.DotNet.Types
{
    public class DotNetSequenceType : SequenceTypeLazyLoaded
    {
        private readonly TypeToolingEnvironment _typeToolingEnvironment;
        private readonly DotNetRawType _rawType;
        private readonly DotNetRawType _elementRawType;

        public DotNetSequenceType(TypeToolingEnvironment typeToolingEnvironment, DotNetRawType rawType, DotNetRawType elementRawType)
        {
            _typeToolingEnvironment = typeToolingEnvironment;
            _rawType = rawType;
            _elementRawType = elementRawType;
        }

        public override RawType UnderlyingType => _rawType;

        public DotNetRawType ElementRawType => _elementRawType;

        protected override CollectionTypeData DoGetData()
        {
            TypeToolingType elementType = _typeToolingEnvironment.GetRequiredType(_elementRawType);
            return new CollectionTypeData(_rawType.FullName, elementType);
        }
    }
}
