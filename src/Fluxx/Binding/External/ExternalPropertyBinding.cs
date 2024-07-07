using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Fluxx.Api;
using TypeTooling.ClassifiedText;
using TypeTooling.Types;

namespace Fluxx.Binding.External
{
    public class ExternalPropertyBinding : PropertyBinding
    {
        private readonly ExternalObjectTypeBinding objectTypeBinding;
        private readonly ObjectProperty objectProperty;
        private readonly TypeBinding propertyTypeBinding;

        public ExternalPropertyBinding(ExternalObjectTypeBinding objectTypeBinding, ObjectProperty objectProperty)
        {
            this.objectTypeBinding = objectTypeBinding;
            this.objectProperty = objectProperty;

            this.propertyTypeBinding = ExternalBindingUtil.TypeToolingTypeToTypeBinding(objectTypeBinding.Project, objectProperty.Type);
        }

        public override TypeBinding GetTypeBinding()
        {
            return this.propertyTypeBinding;
        }

        public override Name PropertyName => new Name(this.objectProperty.Name);

        public override TypeBinding ObjectTypeBinding => this.objectTypeBinding;

        public override Task<ClassifiedTextMarkup?> GetDescriptionAsync(CancellationToken cancellationToken) =>
            this.objectProperty.GetDescriptionAsync(cancellationToken);

        public ObjectType ObjectType => this.objectTypeBinding.TypeToolingType;

        public ObjectProperty ObjectProperty => this.objectProperty;
    }
}
