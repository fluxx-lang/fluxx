using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Fluxx.Api;
using TypeTooling.ClassifiedText;

namespace Fluxx.Binding
{
    public abstract class PropertyBinding : SymbolBinding
    {
        public abstract Name PropertyName { get; }

        public abstract TypeBinding ObjectTypeBinding { get; }

        public abstract Task<ClassifiedTextMarkup?> GetDescriptionAsync(CancellationToken cancellationToken);
    }
}
