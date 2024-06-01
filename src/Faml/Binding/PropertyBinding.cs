
/**
 * @author Bret Johnson
 * @since 4/15/2015
 */

using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Faml.Api;
using TypeTooling.ClassifiedText;

namespace Faml.Binding
{
    public abstract class PropertyBinding : SymbolBinding {
        public abstract Name PropertyName { get; }

        public abstract TypeBinding ObjectTypeBinding { get; }

        public abstract Task<ClassifiedTextMarkup?> GetDescriptionAsync(CancellationToken cancellationToken);
    }
}
