// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System;
using System.ComponentModel;

namespace TypeTooling.Xaml.TypeConverters.UwpWpf
{
    // Converter which allows any string. Used for types with no default constructor like Win8+ FontFamily
    internal class AnyStringConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string));
        }

        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            return (value == null || value is string);
        }
    }
}
