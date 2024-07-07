using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Faml.Api;
using TypeTooling.ClassifiedText;
using TypeTooling.Types;

namespace Faml.Binding.External
{
    public class ExternalPropertyBinding : PropertyBinding
    {
        private readonly ExternalObjectTypeBinding _objectTypeBinding;
        private readonly ObjectProperty _objectProperty;
        private readonly TypeBinding _propertyTypeBinding;


        public ExternalPropertyBinding(ExternalObjectTypeBinding objectTypeBinding, ObjectProperty objectProperty)
        {
            this._objectTypeBinding = objectTypeBinding;
            this._objectProperty = objectProperty;

            this._propertyTypeBinding = ExternalBindingUtil.TypeToolingTypeToTypeBinding(objectTypeBinding.Project, objectProperty.Type);
        }

        public override TypeBinding GetTypeBinding()
        {
            return this._propertyTypeBinding;
        }

        public override Name PropertyName => new Name(this._objectProperty.Name);

        public override TypeBinding ObjectTypeBinding => this._objectTypeBinding;

        public override Task<ClassifiedTextMarkup?> GetDescriptionAsync(CancellationToken cancellationToken) =>
            this._objectProperty.GetDescriptionAsync(cancellationToken);

        public ObjectType ObjectType => this._objectTypeBinding.TypeToolingType;

        public ObjectProperty ObjectProperty => this._objectProperty;
    }
}
