// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System;
using System.ComponentModel;

namespace TypeTooling.Xaml.TypeConverters.UwpWpf
{
    public class UwpWpfImageSourceConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string) || sourceType == typeof(Uri))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            Uri? imageUri = value as Uri;
            if (imageUri == null)
            {
                string? imageUriString = value as string;
                if (!String.IsNullOrEmpty(imageUriString))
                {
                    Uri.TryCreate(imageUriString, UriKind.RelativeOrAbsolute, out imageUri);
                }
            }

            return (imageUri != null);
        }
    }
}
