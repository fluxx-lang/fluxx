using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TypeTooling.DotNet.RawTypes;
using TypeTooling.RawTypes;

namespace Faml.DotNet
{
    public abstract class DotNetRawTypeProvider
    {
        /// <summary>
        /// The provider should return false here to indicate that it's not yet ready to supply type info
        /// (e.g. Roslyn is still reading in project info), in which case the FAML tooling won't call
        /// GetType or the other methods here, instead having a smart fallback for type info 
        /// 
        /// </summary>
        public abstract bool IsReady { get; }

        public abstract DotNetRawType? GetType(string typeName);

        public abstract IReadOnlyList<DotNetRawType> GetAssemblyAttributeReferencedTypes(string attributeFullName);

        public abstract object Instantiate(DotNetRawType type, params object[] args);

        public abstract Task<IEnumerable<DotNetRawType>> FindTypesAssignableToAsync(DotNetRawType rawType, CancellationToken cancellationToken);
    }
}
