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
            this.RawProperty = rawProperty;
            this.Type = type;
        }

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
