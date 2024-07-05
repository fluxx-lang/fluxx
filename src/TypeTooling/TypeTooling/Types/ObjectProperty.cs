using TypeTooling.ClassifiedText;

namespace TypeTooling.Types
{
    public abstract class ObjectProperty
    {
        public ObjectType ObjectType { get;  }

        protected ObjectProperty(ObjectType objectType) {
            ObjectType = objectType;
        }

        public abstract string Name { get; }

        public abstract TypeToolingType Type { get; }

        public abstract bool CanRead { get; }

        public abstract bool CanWrite { get; }

        public abstract Task<ClassifiedTextMarkup?> GetDescriptionAsync(CancellationToken cancellationToken);

#if false
        /// <summary>
        /// Gets the possible sources of values for this property.
        /// </summary>
        ValueSources ValueSources { get; }

        IReadOnlyList<PropertyVariation> Variations { get; }

        IReadOnlyList<IAvailabilityConstraint> AvailabilityConstraints { get; }
#endif
    }
}
