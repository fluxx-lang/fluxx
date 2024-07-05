using System;
using TypeTooling.Types;

namespace TypeTooling.Helper
{
    public abstract class SequenceTypeLazyLoaded : SequenceType
    {
        private readonly Lazy<CollectionTypeData> _data;

        protected SequenceTypeLazyLoaded()
        {
            this._data = new Lazy<CollectionTypeData>(this.DoGetData);
        }

        /// <inheritdoc />
        public sealed override string FullName => this._data.Value.FullName;

        public override TypeToolingType ElementType => this._data.Value.ElementType;

        protected abstract CollectionTypeData DoGetData();
    }

    public sealed class CollectionTypeData
    {
        public string FullName { get; }

        public TypeToolingType ElementType { get; }

        /// <inheritdoc />
        public CollectionTypeData(string fullName, TypeToolingType elementType)
        {
            this.FullName = fullName;
            this.ElementType = elementType;
        }
    }
}
