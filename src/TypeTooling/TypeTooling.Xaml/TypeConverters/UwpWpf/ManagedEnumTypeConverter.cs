// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using TypeTooling.DotNet.RawTypes;

namespace TypeTooling.Xaml.TypeConverters.UwpWpf
{
    /// <summary>
    /// Implements type converter based on Roslyn symbol by using static fields or properties. Suitable
    /// for enum types and enum-like types with the set of static properties, e.g. like FontWeights.
    /// </summary>
    [DebuggerDisplay("{Symbol.Name}")]
    internal class ManagedEnumTypeConverter : TypeConverter
    {
        private List<string>? standardValues;
        private readonly bool supportsEnumNumericValues;
        private readonly bool isFlags;

        public ManagedEnumTypeConverter(DotNetRawType symbol, bool supportsEnumNumericValues)
        {
            this.Symbol = symbol;

            // For enum types we need extra info
            if (this.Symbol.IsEnum)
            {
                this.supportsEnumNumericValues = supportsEnumNumericValues;
                // TODO: Test this
                this.isFlags = this.Symbol.HasAttributeOfType("System.FlagsAttribute");
            }
        }

        public DotNetRawType Symbol { get; private set; }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            // This implementation is needed for Markup.dll XAML parser.
            // We will use value as is which is sufficient for creating
            // document nodes and storing values.
            if (this.IsValid(context, value))
            {
                return value;
            }

            // EnumConverter throws FormatException for values it does not recognize.
            throw new FormatException();
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext? context)
        {
            this.InitializeStandardValuesIfNeeded();
            return new StandardValuesCollection(this.standardValues);
        }

        private void InitializeStandardValuesIfNeeded()
        {
            if (this.standardValues == null)
            {
                List<string> values = new List<string>();
                // This was the original Roslyn based version of the code
                /*
                IEnumerable<ISymbol> members = this.Symbol.GetMembers();
                foreach (ISymbol member in members)
                {
                    if (member.IsStatic && member.HasAccess(MemberAccessTypes.Public))
                    {
                        if (member.Kind == SymbolKind.Field)
                        {
                            values.Add(member.Name);
                        }
                        else if (member.Kind == SymbolKind.Property && this.Symbol.TypeKind == TypeKind.Class)
                        {
                            // For classes like FontWeights we'll use property names too.
                            values.Add(member.Name);
                        }
                    }
                }
                */

                foreach (DotNetRawField member in this.Symbol.GetPublicFields())  
                {
                    if (member.IsStatic && member.IsPublic)
                        values.Add(member.Name);
                }

                foreach (DotNetRawProperty member in this.Symbol.GetPublicProperties())
                {
                    if (member.PropertyType.IsClass)
                        values.Add(member.Name);
                }

                // It is okay to override standardValues in case of a
                // race condition. List of values will be the same.
                this.standardValues = values;
            }
        }

        public override bool IsValid(ITypeDescriptorContext? context, object value)
        {
            // Non-WPF platforms support int values in place of enums, e.g. Visibility="0".
            // Currently we allow Int32 values.
            if (this.supportsEnumNumericValues)
            {
                TypeConverter intConverter = TypeDescriptor.GetConverter(typeof(Int32));
                if (intConverter.IsValid(context, value))
                {
                    return true;
                }
            }

            string? valueAsString = value as string;
            if (string.IsNullOrEmpty(valueAsString))
            {
                return false;
            }

            if (this.IsEnumValue(valueAsString))
            {
                return true;
            }

            // Enums marked as [Flags] can have multiple values, e.g.
            //   ManipulationMode="Scale,TranslateX,TranslateY"
            if (this.isFlags)
            {
                string[] values = valueAsString.Split(',');
                if (values.Length <= 1)
                {
                    // Single valid value is handled above.
                    return false;
                }

                if (values.All(val => this.IsEnumValue(val)))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsEnumValue(string? enumValue)
        {
            this.InitializeStandardValuesIfNeeded();

            // Find enum value matching string. Case insensitive, i.e. following are okay:
            //   Visibility="visible"
            //   Visibility="Visible"
            //   Visibility="VISIBLE"
            string? normalizedValue = enumValue != null ? enumValue.Trim() : null;
            if (!string.IsNullOrEmpty(normalizedValue))
            {
                string standardValue = this.standardValues!.Find(value => value.Equals(normalizedValue, StringComparison.OrdinalIgnoreCase));
                return (standardValue != null);
            }
            else
            {
                return false;
            }
        }
    }
}
