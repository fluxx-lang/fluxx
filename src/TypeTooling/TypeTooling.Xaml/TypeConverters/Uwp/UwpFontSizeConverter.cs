// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Globalization;

namespace TypeTooling.Xaml.TypeConverters.Uwp
{
    public class UwpFontSizeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            // UWP and 8.1 trim the string in order to get a value that is compatible to double.
            // In order to make Designer and Runtime look the same, we need to mimic their behavior.
            string? stringValue = value as string;
            while (!string.IsNullOrEmpty(stringValue))
            {
                if (double.TryParse(stringValue, NumberStyles.Float | NumberStyles.AllowThousands, culture, out double tryDouble))
                {
                    return tryDouble;
                }

                stringValue = stringValue.Substring(0, stringValue.Length - 1);
            }

            return null;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return value.ToString();
        }
    }
}
