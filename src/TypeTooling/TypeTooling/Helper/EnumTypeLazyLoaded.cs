using System;
using System.Collections.Generic;
using TypeTooling.Types;

namespace TypeTooling.Helper
{
    public abstract class EnumTypeLazyLoaded : EnumType
    {
        private readonly Lazy<EnumTypeData> _data;

        protected EnumTypeLazyLoaded()
        {
            _data = new Lazy<EnumTypeData>(DoGetData);
        }

        /// <inheritdoc />
        public sealed override string FullName => _data.Value.FullName;

        /// <inheritdoc />
        public sealed override IReadOnlyCollection<EnumValue> Values => _data.Value.Values;

        /// <inheritdoc />
        protected abstract EnumTypeData DoGetData();
    }

    public sealed class EnumTypeData
    {
        public string FullName { get; }

        public IReadOnlyCollection<EnumValue> Values { get; }

        /// <inheritdoc />
        public EnumTypeData(string fullName, IReadOnlyCollection<EnumValue> values)
        {
            FullName = fullName;
            Values = values;
        }
    }
}