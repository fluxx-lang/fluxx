/**
 * @author Bret Johnson
 * @since 4/15/2015
 */

using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Faml.Api;
using Faml.Syntax;
using TypeTooling.ClassifiedText;

namespace Faml.Binding.Internal {
    public class RecordPropertyBinding : PropertyBinding {
        private readonly RecordTypeDefinitionSyntax _recordTypeDefinition;
        private readonly Name _propertyName;
        private readonly TypeBinding _propertyTypeBinding;

        public RecordPropertyBinding(RecordTypeDefinitionSyntax recordTypeDefinition, Name propertyName) {
            this._recordTypeDefinition = recordTypeDefinition;
            this._propertyName = propertyName;

            this._propertyTypeBinding = this._recordTypeDefinition.GetPropertyTypeBinding(propertyName);
        }

        public override TypeBinding GetTypeBinding() {
            return this._propertyTypeBinding;
        }

        public override Name PropertyName => this._propertyName;

        public override TypeBinding ObjectTypeBinding => this._recordTypeDefinition.TypeBinding;

        public override Task<ClassifiedTextMarkup?> GetDescriptionAsync(CancellationToken cancellationToken) =>
            Task.FromResult((ClassifiedTextMarkup?) null);
    }
}
