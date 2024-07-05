using TypeTooling.DotNet.RawTypes;
using TypeTooling.Helper;
using TypeTooling.RawTypes;
using TypeTooling.Types;

namespace TypeTooling.DotNet.Types
{
    public class DotNetSequenceType : SequenceTypeLazyLoaded
    {
        private readonly TypeToolingEnvironment typeToolingEnvironment;
        private readonly DotNetRawType rawType;
        private readonly DotNetRawType elementRawType;

        public DotNetSequenceType(TypeToolingEnvironment typeToolingEnvironment, DotNetRawType rawType, DotNetRawType elementRawType)
        {
            this.typeToolingEnvironment = typeToolingEnvironment;
            this.rawType = rawType;
            this.elementRawType = elementRawType;
        }

        public override RawType UnderlyingType => this.rawType;

        public DotNetRawType ElementRawType => this.elementRawType;

        protected override CollectionTypeData DoGetData()
        {
            TypeToolingType elementType = this.typeToolingEnvironment.GetRequiredType(this.elementRawType);
            return new CollectionTypeData(this.rawType.FullName, elementType);
        }
    }
}
