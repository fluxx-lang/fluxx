using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using TypeTooling.ClassifiedText;
using TypeTooling.RawTypes;

namespace TypeTooling.Types
{
    public abstract class TypeToolingType
    {
        /// <summary>
        /// Get the fully qualified name for the type, using a "." to delimit any name qualifiers.
        /// </summary>
        public abstract string FullName { get; }

        public abstract RawType UnderlyingType { get; }

        public virtual CustomLiteralParser? GetCustomLiteralParser()
        {
            return null;
        }

        public virtual Visualizer? GetVisualizer()
        {
            return null;
        }

        public virtual Task<ClassifiedTextMarkup?> GetDescriptionAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult((ClassifiedTextMarkup?)null);
        }
    }
}
