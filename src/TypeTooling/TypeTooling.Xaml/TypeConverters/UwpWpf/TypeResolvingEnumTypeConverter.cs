// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using TypeTooling.DotNet.RawTypes;

namespace TypeTooling.Xaml.TypeConverters.UwpWpf
{
    /// <summary>
    /// Type converter which uses type resolver to get a type symbol and create ManagedEnumTypeConverter.
    /// </summary>
    [DebuggerDisplay("{this.IsInitialized ? this.sourceConverter.Symbol.Name : \"uninitialized\"}")]
    internal abstract class TypeResolvingEnumTypeConverter : TypeConverter
    {
        private ManagedEnumTypeConverter? sourceConverter;
        private readonly XamlTypeToolingProvider xamlTypeToolingProvider;

        protected TypeResolvingEnumTypeConverter(XamlTypeToolingProvider xamlTypeToolingProvider)
        {
            this.xamlTypeToolingProvider = xamlTypeToolingProvider;
        }

        protected abstract string GetTypeSymbol();

        protected bool IsInitialized { get { return this.sourceConverter != null; } }

        public Platform Platform => xamlTypeToolingProvider.Platform;

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return this.IsInitialized && this.sourceConverter!.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (this.IsInitialized)
            {
                return this.sourceConverter!.ConvertFrom(context, culture, value);
            }
            else
            {
                // EnumConverter throws FormatException for values it does not recognize.
                throw new FormatException();
            }
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return this.IsInitialized && this.sourceConverter!.GetStandardValuesSupported(context);
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return this.IsInitialized && this.sourceConverter!.GetStandardValuesExclusive(context);
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (this.IsInitialized)
            {
                return this.sourceConverter!.GetStandardValues(null);
            }

            return new StandardValuesCollection(new List<object>());
        }

        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            return this.IsInitialized && this.sourceConverter!.IsValid(null, value);
        }

        public void Initialize()
        {
            if (this.sourceConverter != null)
            {
                return;	// already initialized
            }

            string typeSymbol = this.GetTypeSymbol();
            var type = (DotNetRawType?) xamlTypeToolingProvider.TypeToolingEnvironment.GetRawType(typeSymbol);

            if (type == null) {
                // This shouldn't happen
                return;
            }

            bool supportsEnumNumericValues = Platform == Platform.Uwp;
            this.sourceConverter = new ManagedEnumTypeConverter(type, supportsEnumNumericValues);
        }
    }
}
