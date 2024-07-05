/**
 * @author Bret Johnson
 * @since 4/15/2015
 */

using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Faml.Api;
using TypeTooling.ClassifiedText;
using TypeTooling.Types;

namespace Faml.Binding.External {
    public class ExternalPropertyBinding : PropertyBinding {
        private readonly ExternalObjectTypeBinding _objectTypeBinding;
        private readonly ObjectProperty _objectProperty;
        private readonly TypeBinding _propertyTypeBinding;


        public ExternalPropertyBinding(ExternalObjectTypeBinding objectTypeBinding, ObjectProperty objectProperty) {
            _objectTypeBinding = objectTypeBinding;
            _objectProperty = objectProperty;

            _propertyTypeBinding = ExternalBindingUtil.TypeToolingTypeToTypeBinding(objectTypeBinding.Project, objectProperty.Type);
        }

        public override TypeBinding GetTypeBinding() {
            return _propertyTypeBinding;
        }

        public override Name PropertyName => new Name(_objectProperty.Name);

        public override TypeBinding ObjectTypeBinding => _objectTypeBinding;

        public override Task<ClassifiedTextMarkup?> GetDescriptionAsync(CancellationToken cancellationToken) =>
            _objectProperty.GetDescriptionAsync(cancellationToken);

        public ObjectType ObjectType => _objectTypeBinding.TypeToolingType;

        public ObjectProperty ObjectProperty => _objectProperty;
    }
}
