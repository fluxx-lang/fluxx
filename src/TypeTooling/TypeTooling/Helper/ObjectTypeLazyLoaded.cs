using System;
using System.Collections.Generic;
using TypeTooling.Types;

namespace TypeTooling.Helper
{
    public abstract class ObjectTypeLazyLoaded : ObjectType
    {
        private readonly Lazy<ObjectTypeData> data;

        protected ObjectTypeLazyLoaded()
        {
            this.data = new Lazy<ObjectTypeData>(this.DoGetData);
        }

        /// <inheritdoc />
        public sealed override string FullName => this.data.Value.FullName;

        /// <inheritdoc />
        public sealed override IReadOnlyCollection<ObjectProperty> Properties => this.data.Value.Properties;

        /// <inheritdoc />
        public sealed override ObjectProperty? ContentProperty => this.data.Value.ContentProperty;

        /// <inheritdoc />
        public override CustomLiteralParser? GetCustomLiteralParser()
        {
            return this.data.Value.CustomLiteralParser;
        }

        public override IReadOnlyCollection<ObjectType> GetBaseTypes()
        {
            return this.data.Value.BaseTypes;
        }

        protected abstract ObjectTypeData DoGetData();
    }

    public sealed class ObjectTypeData
    {
        public string FullName { get; }

        public IReadOnlyCollection<ObjectProperty> Properties { get; }

        public ObjectProperty? ContentProperty { get; }

        public CustomLiteralParser? CustomLiteralParser { get; }

        public IReadOnlyCollection<ObjectType> BaseTypes { get; }

        /// <inheritdoc />
        public ObjectTypeData(string fullName, IReadOnlyCollection<ObjectProperty> properties,
            ObjectProperty? contentProperty, CustomLiteralParser? customLiteralParser,
            IReadOnlyCollection<ObjectType> baseTypes)
            {
            this.FullName = fullName;
            this.Properties = properties;
            this.ContentProperty = contentProperty;
            this.CustomLiteralParser = customLiteralParser;
            this.BaseTypes = baseTypes;
        }
    }
}
