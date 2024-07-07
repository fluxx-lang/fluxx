using TypeTooling.ClassifiedText;
using TypeTooling.DotNet.RawTypes;
using TypeTooling.Types;

namespace TypeTooling.DotNet.Types
{
    public class DotNetObjectProperty(ObjectType objectType, DotNetRawProperty rawProperty, TypeToolingType type) : ObjectProperty(objectType)
    {
        // Automatic properties
        public DotNetRawProperty RawProperty { get; } = rawProperty;

        public override TypeToolingType Type { get; } = type;

        public override string Name => this.RawProperty.Name;

        public override bool CanRead => this.RawProperty.CanRead;

        public override bool CanWrite => this.RawProperty.CanWrite;

        public DotNetObjectType DotNetObjectType => (DotNetObjectType)this.ObjectType;

        public override Task<ClassifiedTextMarkup?> GetDescriptionAsync(CancellationToken cancellationToken)
        {
            return this.RawProperty.GetDescriptionAsync(this.DotNetObjectType.TypeToolingEnvironment.UICulture, cancellationToken);
        }
    }
}
