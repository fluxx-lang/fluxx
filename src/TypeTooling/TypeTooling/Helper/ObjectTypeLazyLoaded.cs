using System;
using System.Collections.Generic;
using TypeTooling.Types;

namespace TypeTooling.Helper {
    public abstract class ObjectTypeLazyLoaded : ObjectType {
        private readonly Lazy<ObjectTypeData> _data;

        protected ObjectTypeLazyLoaded() {
            _data = new Lazy<ObjectTypeData>(DoGetData);
        }

        /// <inheritdoc />
        public sealed override string FullName => _data.Value.FullName;

        /// <inheritdoc />
        public sealed override IReadOnlyCollection<ObjectProperty> Properties => _data.Value.Properties;

        /// <inheritdoc />
        public sealed override ObjectProperty? ContentProperty => _data.Value.ContentProperty;

        /// <inheritdoc />
        public override CustomLiteralParser? GetCustomLiteralParser() {
            return _data.Value.CustomLiteralParser;
        }

        public override IReadOnlyCollection<ObjectType> GetBaseTypes() {
            return _data.Value.BaseTypes;
        }

        protected abstract ObjectTypeData DoGetData();
    }

    public sealed class ObjectTypeData {
        public string FullName { get; }

        public IReadOnlyCollection<ObjectProperty> Properties { get; }

        public ObjectProperty? ContentProperty { get; }

        public CustomLiteralParser? CustomLiteralParser { get; }

        public IReadOnlyCollection<ObjectType> BaseTypes { get; }

        /// <inheritdoc />
        public ObjectTypeData(string fullName, IReadOnlyCollection<ObjectProperty> properties,
            ObjectProperty? contentProperty, CustomLiteralParser? customLiteralParser,
            IReadOnlyCollection<ObjectType> baseTypes) {
            FullName = fullName;
            Properties = properties;
            ContentProperty = contentProperty;
            CustomLiteralParser = customLiteralParser;
            BaseTypes = baseTypes;
        }
    }
}
