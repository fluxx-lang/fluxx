using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Fluxx.Api;
using TypeTooling.ClassifiedText;

namespace Fluxx.Binding
{
    public class InvalidPropertyBinding : PropertyBinding
    {
        private readonly Name propertyName;

        public InvalidPropertyBinding(Name propertyName)
        {
            this.propertyName = propertyName;
        }

        public override TypeBinding GetTypeBinding()
        {
            return InvalidTypeBinding.Instance;
        }

        public override Name PropertyName => this.propertyName;

        public override TypeBinding ObjectTypeBinding => InvalidTypeBinding.Instance;

        public override Task<ClassifiedTextMarkup?> GetDescriptionAsync(CancellationToken cancellationToken) =>
            Task.FromResult((ClassifiedTextMarkup?)null);
    }
}
