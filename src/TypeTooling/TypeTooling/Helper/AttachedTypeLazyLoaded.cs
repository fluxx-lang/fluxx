using System;
using System.Collections.Generic;
using TypeTooling.Types;

namespace TypeTooling.Helper
{
    public abstract class AttachedTypeLazyLoaded : AttachedType
    {
        private readonly Lazy<AttachedTypeData> _data;

        protected AttachedTypeLazyLoaded()
        {
            _data = new Lazy<AttachedTypeData>(DoGetData);
        }

        /// <inheritdoc />
        public sealed override string FullName => _data.Value.FullName;

        /// <inheritdoc />
        public sealed override IReadOnlyCollection<AttachedProperty> AttachedProperties => _data.Value.AttachedProperties;

        /// <inheritdoc />
        public override IReadOnlyCollection<AttachedType> GetBaseTypes()
        {
            return _data.Value.BaseTypes;
        }

        protected abstract AttachedTypeData DoGetData();
    }

    public sealed class AttachedTypeData
    {
        public string FullName { get; }

        public IReadOnlyCollection<AttachedProperty> AttachedProperties { get; }

        public IReadOnlyCollection<AttachedType> BaseTypes { get; }

        public AttachedTypeData(string fullName, IReadOnlyCollection<AttachedProperty> attachedProperties, IReadOnlyCollection<AttachedType> baseTypes)
        {
            FullName = fullName;
            AttachedProperties = attachedProperties;
            BaseTypes = baseTypes;
        }
    }
}
