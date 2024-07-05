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
            this._typeToolingEnvironment = typeToolingEnvironment;
            this._rawType = rawType;
            this._elementRawType = elementRawType;
        }

        public override RawType UnderlyingType => this._rawType;

        public DotNetRawType ElementRawType => this._elementRawType;

        protected override CollectionTypeData DoGetData()
        {
            TypeToolingType elementType = this._typeToolingEnvironment.GetRequiredType(this._elementRawType);
            return new CollectionTypeData(this._rawType.FullName, elementType);
        }
    }
}
