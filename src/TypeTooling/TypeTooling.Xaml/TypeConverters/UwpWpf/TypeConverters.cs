// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using TypeTooling;
using TypeTooling.CodeGeneration;
using TypeTooling.CodeGeneration.Expressions;
using TypeTooling.DotNet.CodeGeneration;
using TypeTooling.DotNet.RawTypes;
using TypeTooling.Types;

namespace TypeTooling.Xaml.TypeConverters.UwpWpf
{
    struct UwpWpfBrushStub {
        private UwpWpfColor _color;

        public UwpWpfBrushStub(UwpWpfColor color) {
            _color = color;
        }
    }

    public class UwpWpfBrushParser : XamlCustomLiteralParser
    {
        public UwpWpfBrushParser(XamlTypeToolingProvider typeToolingProvider) : base(typeToolingProvider) {
        }

        public override CustomLiteral Parse(string literal) {
            UwpWpfColorCustomLiteralParser colorParser = new UwpWpfColorCustomLiteralParser(XamlTypeToolingProvider);

            CustomLiteral customLiteral = colorParser.Parse(literal);
            if (customLiteral.ExpressionAndHelpersCode == null)
                return customLiteral;

            try {
                ExpressionCode colorExpression = customLiteral.ExpressionAndHelpersCode.Expression;

                DotNetRawType solidColorBrushType =
                    XamlTypeToolingProvider.GetRequiredRawType("Windows.UI.Xaml.Media.SolidColorBrush");

                ExpressionCode brushExpression = DotNetCode.New(solidColorBrushType, new[] { "Windows.UI.Color" }, colorExpression);
                //ExpressionCode brushExpression = DotNetCode.New(solidColorBrushType);

                return new CustomLiteral(brushExpression);
            }
            catch (UserViewableException e) {
                return CustomLiteral.SingleError(e.Message);
            }
            catch (Exception) {
                return CustomLiteral.SingleError("Invalid value");
            }
        }

        public override IReadOnlyCollection<string> GetCommonValues(ITypeDescriptorContext context) {
            var colorConverter = new UwpWpfColorCustomLiteralParser(XamlTypeToolingProvider);
            return colorConverter.GetCommonValues(context);
        }
    }

    // The class from StaticStandardValuesTypeConverters is used instead
#if WPF_ONLY
    public class CacheModeConverter : TypeConverter
    {
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(value, attributes);
        }
    }
#endif

    struct UwpWpfColor {
        public byte A;
        public byte R;
        public byte G;
        public byte B;

        public static UwpWpfColor FromArgb(byte a, byte r, byte g, byte b) {
            return new UwpWpfColor {
                A = a,
                R = r,
                G = g,
                B = b
            };
        }
    }

    public class UwpWpfColorCustomLiteralParser : XamlCustomLiteralParser
    {
        private readonly string? _initializationError;
        private readonly DotNetRawType? _colorHelperType;
        private readonly DotNetRawType? _byteType;


        public UwpWpfColorCustomLiteralParser(XamlTypeToolingProvider xamlTypeToolingProvider) : base(xamlTypeToolingProvider) {
            try {
                _colorHelperType = XamlTypeToolingProvider.GetRequiredRawType("Windows.UI.ColorHelper");
                _byteType = XamlTypeToolingProvider.GetRequiredRawType("System.Byte");
            }
            catch (UserViewableException e) {
                _initializationError = "GridLength custom literal initialization error: " + e.Message;
            }
        }

        public override CustomLiteral Parse(string literal) {
            if (_initializationError != null)
                return CustomLiteral.SingleError(_initializationError);

            try {
                if (!StockColorsByName.TryGetValue(literal, out UwpWpfColor color))
                    color = UwpWpfColorParser.ParseColor(literal, CultureInfo.InvariantCulture, null);

                /*
                if (value.Equals("Fuchsia", StringComparison.InvariantCultureIgnoreCase))
                    return Fuchsia;
                else if (value.Equals("Aqua", StringComparison.InvariantCultureIgnoreCase))
                    return Aqua;
                */

                MethodCallCode expression = DotNetCode.CallStatic(_colorHelperType!, "FromArgb",
                    new[] { _byteType!, _byteType!, _byteType!, _byteType!},
                    Code.ByteLiteral(color.A), Code.ByteLiteral(color.R), Code.ByteLiteral(color.G), Code.ByteLiteral(color.B));

                return new CustomLiteral(expression);
            }
            catch (UserViewableException e) {
                return CustomLiteral.SingleError(e.Message);
            }
            catch (Exception) {
                return CustomLiteral.SingleError("Invalid value");
            }
        }

        // Since Magenta and Fuchsia and Cyan and Aqua have the same color values, 
        // we create a slightly off color version to allow them to show up in the
        // drop-down list. This also allows us to preserve the user choice of name.
        // This doens't affect runtime (unless someone really wanted the 01 versions)
        // since the runtime will convert these values to the correct, non-off-color
        // versions.
        internal static UwpWpfColor Fuchsia = UwpWpfColor.FromArgb(0xFF, 0xFF, 0x01, 0xFF);
        internal static UwpWpfColor Aqua = UwpWpfColor.FromArgb(0xFF, 0x01, 0xFF, 0xFF);
        private static readonly Dictionary<uint, string> StockColors = new Dictionary<uint, string>()
        {
            {0xFFF0F8FF, "AliceBlue"},
            {0xFFFAEBD7, "AntiqueWhite"},
            {0xFF01FFFF, "Aqua"},         // Use a slightly off-color version of Cyan to encode Aqua
            {0xFF7FFFD4, "Aquamarine"},
            {0xFFF0FFFF, "Azure"},
            {0xFFF5F5DC, "Beige"},
            {0xFFFFE4C4, "Bisque"},
            {0xFF000000, "Black"},
            {0xFFFFEBCD, "BlanchedAlmond"},
            {0xFF0000FF, "Blue"},
            {0xFF8A2BE2, "BlueViolet"},
            {0xFFA52A2A, "Brown"},
            {0xFFDEB887, "BurlyWood"},
            {0xFF5F9EA0, "CadetBlue"},
            {0xFF7FFF00, "Chartreuse"},
            {0xFFD2691E, "Chocolate"},
            {0xFFFF7F50, "Coral"},
            {0xFF6495ED, "CornflowerBlue"},
            {0xFFFFF8DC, "Cornsilk"},
            {0xFFDC143C, "Crimson"},
            {0xFF00FFFF, "Cyan"},
            {0xFF00008B, "DarkBlue"},
            {0xFF008B8B, "DarkCyan"},
            {0xFFB8860B, "DarkGoldenrod"},
            {0xFFA9A9A9, "DarkGray"},
            {0xFF006400, "DarkGreen"},
            {0xFFBDB76B, "DarkKhaki"},
            {0xFF8B008B, "DarkMagenta"},
            {0xFF556B2F, "DarkOliveGreen"},
            {0xFFFF8C00, "DarkOrange"},
            {0xFF9932CC, "DarkOrchid"},
            {0xFF8B0000, "DarkRed"},
            {0xFFE9967A, "DarkSalmon"},
            {0xFF8FBC8F, "DarkSeaGreen"},
            {0xFF483D8B, "DarkSlateBlue"},
            {0xFF2F4F4F, "DarkSlateGray"},
            {0xFF00CED1, "DarkTurquoise"},
            {0xFF9400D3, "DarkViolet"},
            {0xFFFF1493, "DeepPink"},
            {0xFF00BFFF, "DeepSkyBlue"},
            {0xFF696969, "DimGray"},
            {0xFF1E90FF, "DodgerBlue"},
            {0xFFB22222, "Firebrick"},
            {0xFFFFFAF0, "FloralWhite"},
            {0xFF228B22, "ForestGreen"},
            {0xFFFF01FF, "Fuchsia"},      // Use a slightly off-color version of Magenta to encode Fuschia
            {0xFFDCDCDC, "Gainsboro"},
            {0xFFF8F8FF, "GhostWhite"},
            {0xFFFFD700, "Gold"},
            {0xFFDAA520, "Goldenrod"},
            {0xFF808080, "Gray"},
            {0xFF008000, "Green"},
            {0xFFADFF2F, "GreenYellow"},
            {0xFFF0FFF0, "Honeydew"},
            {0xFFFF69B4, "HotPink"},
            {0xFFCD5C5C, "IndianRed"},
            {0xFF4B0082, "Indigo"},
            {0xFFFFFFF0, "Ivory"},
            {0xFFF0E68C, "Khaki"},
            {0xFFE6E6FA, "Lavender"},
            {0xFFFFF0F5, "LavenderBlush"},
            {0xFF7CFC00, "LawnGreen"},
            {0xFFFFFACD, "LemonChiffon"},
            {0xFFADD8E6, "LightBlue"},
            {0xFFF08080, "LightCoral"},
            {0xFFE0FFFF, "LightCyan"},
            {0xFFFAFAD2, "LightGoldenrodYellow"},
            {0xFFD3D3D3, "LightGray"},
            {0xFF90EE90, "LightGreen"},
            {0xFFFFB6C1, "LightPink"},
            {0xFFFFA07A, "LightSalmon"},
            {0xFF20B2AA, "LightSeaGreen"},
            {0xFF87CEFA, "LightSkyBlue"},
            {0xFF778899, "LightSlateGray"},
            {0xFFB0C4DE, "LightSteelBlue"},
            {0xFFFFFFE0, "LightYellow"},
            {0xFF00FF00, "Lime"},
            {0xFF32CD32, "LimeGreen"},
            {0xFFFAF0E6, "Linen"},
            {0xFFFF00FF, "Magenta"},
            {0xFF800000, "Maroon"},
            {0xFF66CDAA, "MediumAquamarine"},
            {0xFF0000CD, "MediumBlue"},
            {0xFFBA55D3, "MediumOrchid"},
            {0xFF9370DB, "MediumPurple"},
            {0xFF3CB371, "MediumSeaGreen"},
            {0xFF7B68EE, "MediumSlateBlue"},
            {0xFF00FA9A, "MediumSpringGreen"},
            {0xFF48D1CC, "MediumTurquoise"},
            {0xFFC71585, "MediumVioletRed"},
            {0xFF191970, "MidnightBlue"},
            {0xFFF5FFFA, "MintCream"},
            {0xFFFFE4E1, "MistyRose"},
            {0xFFFFE4B5, "Moccasin"},
            {0xFFFFDEAD, "NavajoWhite"},
            {0xFF000080, "Navy"},
            {0xFFFDF5E6, "OldLace"},
            {0xFF808000, "Olive"},
            {0xFF6B8E23, "OliveDrab"},
            {0xFFFFA500, "Orange"},
            {0xFFFF4500, "OrangeRed"},
            {0xFFDA70D6, "Orchid"},
            {0xFFEEE8AA, "PaleGoldenrod"},
            {0xFF98FB98, "PaleGreen"},
            {0xFFAFEEEE, "PaleTurquoise"},
            {0xFFDB7093, "PaleVioletRed"},
            {0xFFFFEFD5, "PapayaWhip"},
            {0xFFFFDAB9, "PeachPuff"},
            {0xFFCD853F, "Peru"},
            {0xFFFFC0CB, "Pink"},
            {0xFFDDA0DD, "Plum"},
            {0xFFB0E0E6, "PowderBlue"},
            {0xFF800080, "Purple"},
            {0xFFFF0000, "Red"},
            {0xFFBC8F8F, "RosyBrown"},
            {0xFF4169E1, "RoyalBlue"},
            {0xFF8B4513, "SaddleBrown"},
            {0xFFFA8072, "Salmon"},
            {0xFFF4A460, "SandyBrown"},
            {0xFF2E8B57, "SeaGreen"},
            {0xFFFFF5EE, "SeaShell"},
            {0xFFA0522D, "Sienna"},
            {0xFFC0C0C0, "Silver"},
            {0xFF87CEEB, "SkyBlue"},
            {0xFF6A5ACD, "SlateBlue"},
            {0xFF708090, "SlateGray"},
            {0xFFFFFAFA, "Snow"},
            {0xFF00FF7F, "SpringGreen"},
            {0xFF4682B4, "SteelBlue"},
            {0xFFD2B48C, "Tan"},
            {0xFF008080, "Teal"},
            {0xFFD8BFD8, "Thistle"},
            {0xFFFF6347, "Tomato"},
            {0x00FFFFFF, "Transparent"},
            {0xFF40E0D0, "Turquoise"},
            {0xFFEE82EE, "Violet"},
            {0xFFF5DEB3, "Wheat"},
            {0xFFFFFFFF, "White"},
            {0xFFF5F5F5, "WhiteSmoke"},
            {0xFFFFFF00, "Yellow"},
            {0xFF9ACD32, "YellowGreen"}
        };

        private static readonly Dictionary<string, UwpWpfColor> StockColorsByName = CreateStockColorsByName();

        private static Dictionary<string, UwpWpfColor> CreateStockColorsByName() {
            var dictionary = new Dictionary<string, UwpWpfColor>();
            foreach (var keyValuePair in StockColors) {
                uint colorUint = keyValuePair.Key;

                UwpWpfColor color = UwpWpfColor.FromArgb((byte)((colorUint >> 24) & 0xFF),
                    (byte)((colorUint >> 16) & 0xFF),
                    (byte)((colorUint >> 8) & 0xFF),
                    (byte)(colorUint & 0xFF));

                dictionary.Add(keyValuePair.Value, color);
            }
            return dictionary;
        }


#if false
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value is Color)
                {
                    Color color = (Color)value;
                    if (color.ColorContext == null)
                    {
                        string result = baseConverter.ConvertTo(context, culture, value, destinationType) as string;
                        if (result != null && result.StartsWith("sc(", StringComparison.Ordinal))
                            return result;
                        string canonicalValue = ConvertColor(color);
                        if (canonicalValue != null)
                            result = canonicalValue;
                        return result;
                    }
                }
            }
            return baseConverter.ConvertTo(context, culture, value, destinationType);
        }
#endif

        public override IReadOnlyCollection<string> GetCommonValues(ITypeDescriptorContext context)
        {
            return GetStockColorNames();
        }

        protected internal static IReadOnlyCollection<string> GetStockColorNames()
        {
            return StockColors.Values;
        }
    }

#if WPF_ONLY
    /// <summary>
    /// Convert a Thickness to a string and back. Uses System.Windows.ThicknessConverter but
    /// changes the default behavior to represent 1,1,1,1 as 1 and 1,2,1,2 as 1,2 instead of
    /// the verbose version returned by System.Windows.ThicknessConverter.
    /// </summary>
    public class ThicknessConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return baseConverter.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return baseConverter.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return baseConverter.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value is Thickness)
                {
                    Thickness thickness = (Thickness)value;
                    string result = ConvertThickness(context, culture, thickness);
                    if (result != null)
                        return result;
                }
            }
            return baseConverter.ConvertTo(context, culture, value, destinationType);
        }

        // Also called from ThicknessValueSerializer
        internal static string ConvertThickness(ITypeDescriptorContext context, CultureInfo culture, Thickness thickness)
        {
            if (thickness.Left == thickness.Right && thickness.Top == thickness.Bottom)
            {
                // At least N,M
                string N = lengthConverter.ConvertTo(context, culture, thickness.Left, typeof(string)) as string;
                if (N == null)
                    return null;
                if (thickness.Left == thickness.Top)
                    // Only a N
                    return N;
                else
                {
                    // N,M
                    StringBuilder builder = new StringBuilder(32);
                    char numericListSeparator = GetNumericListSeparator(culture);
                    string M = lengthConverter.ConvertTo(context, culture, thickness.Top, typeof(string)) as string;
                    if (M == null)
                        return null;
                    builder.Append(N);
                    builder.Append(numericListSeparator);
                    builder.Append(M);
                    return builder.ToString();
                }
            }

            return null;
        }

        internal static char GetNumericListSeparator(IFormatProvider provider)
        {
            char ch = ',';
            NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
            if ((instance.NumberDecimalSeparator.Length > 0) && (ch == instance.NumberDecimalSeparator[0]))
            {
                ch = ';';
            }
            return ch;
        }


        static TypeConverter lengthConverter = new System.Windows.LengthConverter();
        static TypeConverter baseConverter = new System.Windows.ThicknessConverter();
    }
#endif

#if WPF_ONLY
    /// <summary>
    /// Convert a CornerRadius to a string and back. Uses System.Windows.CornerRadiusConverter but
    /// changes the default behavior to represent 1,1,1,1 as 1 instead of
    /// the verbose version returned by System.Windows.CornerRadiusConverter.
    /// </summary>
    public class CornerRadiusConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return baseConverter.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return baseConverter.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return baseConverter.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value is CornerRadius)
                {
                    CornerRadius cr = (CornerRadius)value;
                    if (cr.TopLeft == cr.TopRight && cr.TopRight == cr.BottomLeft && cr.BottomLeft == cr.BottomRight)
                    {
                        string N = lengthConverter.ConvertTo(context, culture, cr.TopLeft, typeof(string)) as string;
                        if (N != null)
                            return N;
                    }
                }
            }
            return baseConverter.ConvertTo(context, culture, value, destinationType);
        }

        static TypeConverter lengthConverter = new System.Windows.LengthConverter();
        static TypeConverter baseConverter = new System.Windows.CornerRadiusConverter();
    }
#endif

#if WPF_ONLY
    public class CustomKeyConverter : System.Windows.Input.KeyConverter
    {
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(Enum.GetValues(typeof(System.Windows.Input.Key)));
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
#endif

#if WPF_ONLY
    public class WpfCustomModifierKeysConverter : System.Windows.Input.ModifierKeysConverter
    {
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(Enum.GetValues(typeof(System.Windows.Input.ModifierKeys)));
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
#endif

#if WPF_ONLY
    public class WpfStrokeDashArrayConverter : TypeConverter
    {
        private DoubleCollectionConverter converter = new DoubleCollectionConverter();

        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, Type sourceType)
        {
            return converter.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, Type destinationType)
        {
            return converter.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            return converter.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (typeof(string).IsAssignableFrom(destinationType))
            {
                string result = (string)converter.ConvertTo(context, culture, value, destinationType);
                // For stroke dash array, use this as the default value, to show the basic stroke dash pattern to the user.
                if (string.IsNullOrEmpty(result))
                {
                    result = "1 0";
                }
                return result;
            }
            else
            {
                return converter.ConvertTo(context, culture, value, destinationType);
            }
        }
    }
#endif
}
