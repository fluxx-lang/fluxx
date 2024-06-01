using System.Collections.Generic;

namespace TypeTooling.Types {
    public abstract class AttachedType {
        public abstract string FullName { get; }

        public abstract IReadOnlyCollection<AttachedProperty> AttachedProperties { get; }

        public abstract IReadOnlyCollection<AttachedType> GetBaseTypes();
    }
}
