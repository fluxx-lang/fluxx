using System;
using System.ComponentModel;
using System.Globalization;

// This code came from VS/src/Xaml/Designer/Source/UwpDesigner/Utility/XamlTypeConverters.cs

// The type converters in this file are functional within the context Blend uses them, but are
// not designed for general purpose use.
namespace TypeTooling.Xaml.TypeConverters.Uwp {
    public abstract class ConvertToFromStringConverter : TypeConverter {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            return (sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            string stringValue = value as string;
            if (stringValue != null) {
                return this.ConvertFromStringInternal(stringValue, culture);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public abstract object ConvertFromStringInternal(string value, IFormatProvider formatProvider);

        protected double StringToDouble(string s, IFormatProvider formatProvider, bool supportAuto) {
            if (supportAuto) {
                s = s.ToLowerInvariant();
                if (s == "auto") {
                    return double.NaN;
                }
            }

            return Convert.ToDouble(s, formatProvider);
        }

        protected string DoubleToString(double d, IFormatProvider formatProvider, bool supportAuto) {
            if (supportAuto && double.IsNaN(d)) {
                return "Auto";
            }

            return Convert.ToString(d, formatProvider);
        }

        protected static char[] GetNumericListSeparator(IFormatProvider provider) {
            char ch = ',';
            NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
            if ((instance.NumberDecimalSeparator.Length > 0) && (ch == instance.NumberDecimalSeparator[0])) {
                ch = ';';
            }

            return new char[] {ch, ' ', '\t'};
        }
    }
}