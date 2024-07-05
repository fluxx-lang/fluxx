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
            this._data = new Lazy<EnumTypeData>(this.DoGetData);
        }

        /// <inheritdoc />
        public sealed override string FullName => this._data.Value.FullName;

        /// <inheritdoc />
        public sealed override IReadOnlyCollection<EnumValue> Values => this._data.Value.Values;

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
            this.FullName = fullName;
            this.Values = values;
        }
    }
}