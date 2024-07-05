namespace TypeTooling.Types
{
    public abstract class AttachedProperty
    {
        public AttachedType AttachedType{ get; }

        protected AttachedProperty(AttachedType attachedType)
        {
            this.AttachedType = attachedType;
        }

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
