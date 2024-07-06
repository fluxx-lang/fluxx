namespace TypeTooling.Types
{
    public abstract class AttachedProperty(AttachedType attachedType)
    {
        public AttachedType AttachedType { get; } = attachedType;

        public abstract string Name { get; }

        public abstract TypeToolingType Type { get; }

        public abstract ObjectType TargetObjectType { get; }

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
