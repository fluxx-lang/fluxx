using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using TypeTooling.ClassifiedText;
using TypeTooling.DotNet.RawTypes;
using TypeTooling.Types;

namespace TypeTooling.DotNet.Types
{
    public class DotNetObjectProperty : ObjectProperty
    {
        // Automatic properties
        public DotNetRawProperty RawProperty { get; }
        public override TypeToolingType Type { get; }

        public DotNetObjectProperty(ObjectType objectType, DotNetRawProperty rawProperty, TypeToolingType type) : base(objectType)
        {
            RawProperty = rawProperty;
            Type = type;
        }

        public override string Name => RawProperty.Name;

        public override bool CanRead => RawProperty.CanRead;

        public override bool CanWrite => RawProperty.CanWrite;

        public DotNetObjectType DotNetObjectType => (DotNetObjectType) ObjectType;

        public override Task<ClassifiedTextMarkup?> GetDescriptionAsync(CancellationToken cancellationToken)
        {
            return RawProperty.GetDescriptionAsync(DotNetObjectType.TypeToolingEnvironment.UICulture, cancellationToken);
        }
    }
}
