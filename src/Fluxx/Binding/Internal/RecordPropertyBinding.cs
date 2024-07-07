using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Fluxx.Api;
using Fluxx.Syntax;
using TypeTooling.ClassifiedText;

namespace Fluxx.Binding.Internal
{
    public class RecordPropertyBinding : PropertyBinding
    {
        private readonly RecordTypeDefinitionSyntax recordTypeDefinition;
        private readonly Name propertyName;
        private readonly TypeBinding propertyTypeBinding;

        public RecordPropertyBinding(RecordTypeDefinitionSyntax recordTypeDefinition, Name propertyName)
        {
            this.recordTypeDefinition = recordTypeDefinition;
            this.propertyName = propertyName;

            this.propertyTypeBinding = this.recordTypeDefinition.GetPropertyTypeBinding(propertyName);
        }

        public override TypeBinding GetTypeBinding()
        {
            return this.propertyTypeBinding;
        }

        public override Name PropertyName => this.propertyName;

        public override TypeBinding ObjectTypeBinding => this.recordTypeDefinition.TypeBinding;

        public override Task<ClassifiedTextMarkup?> GetDescriptionAsync(CancellationToken cancellationToken) =>
            Task.FromResult((ClassifiedTextMarkup?)null);
    }
}
