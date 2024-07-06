﻿using System;
using System.Collections.Generic;
using TypeTooling.Types;

namespace TypeTooling.Helper
{
    public abstract class AttachedTypeLazyLoaded : AttachedType
    {
        private readonly Lazy<AttachedTypeData> data;

        protected AttachedTypeLazyLoaded()
        {
            this.data = new Lazy<AttachedTypeData>(this.DoGetData);
        }

        /// <inheritdoc />
        public sealed override string FullName => this.data.Value.FullName;

        /// <inheritdoc />
        public sealed override IReadOnlyCollection<AttachedProperty> AttachedProperties => this.data.Value.AttachedProperties;

        /// <inheritdoc />
        public override IReadOnlyCollection<AttachedType> GetBaseTypes()
        {
            return this.data.Value.BaseTypes;
        }

        protected abstract AttachedTypeData DoGetData();
    }

    public sealed class AttachedTypeData(string fullName, IReadOnlyCollection<AttachedProperty> attachedProperties, IReadOnlyCollection<AttachedType> baseTypes)
    {
        public string FullName { get; } = fullName;

        public IReadOnlyCollection<AttachedProperty> AttachedProperties { get; } = attachedProperties;

        public IReadOnlyCollection<AttachedType> BaseTypes { get; } = baseTypes;
    }
}
