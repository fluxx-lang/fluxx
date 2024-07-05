using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using TypeTooling.ClassifiedText;
using TypeTooling.RawTypes;

namespace TypeTooling.DotNet.RawTypes
{
    public abstract class DotNetRawProperty : RawProperty
    {
        public abstract string Name { get; }

        public abstract DotNetRawType PropertyType { get;  }

        public abstract IEnumerable<DotNetRawCustomAttribute> GetCustomAttributes();

        public abstract bool CanRead { get; }

        public abstract bool CanWrite { get; }

        public abstract Task<ClassifiedTextMarkup?> GetDescriptionAsync(CultureInfo preferredCulture,
            CancellationToken cancellationToken);
    }
}
