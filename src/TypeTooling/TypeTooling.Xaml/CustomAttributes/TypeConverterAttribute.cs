// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System;
using System.ComponentModel;

namespace TypeTooling.Xaml.CustomAttributes
{
    /// <summary>
    /// Specifies what type to use as a converter for the object this attribute is bound to.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public sealed class TypeConverterAttribute : Attribute
    {
        private TypeConverter typeConverter;

        public TypeConverterAttribute(TypeConverter typeConverter)
        {
            this.typeConverter = typeConverter;
        }

        public TypeConverter TypeConverter => typeConverter;
    }
}
