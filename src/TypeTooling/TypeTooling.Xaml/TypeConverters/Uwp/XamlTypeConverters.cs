#if false
// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------
namespace Microsoft.VisualStudio.DesignTools.UwpDesigner.TypeConverters
{
    extern alias WindowsRuntime;

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Windows.Media.Media3D;
    using Microsoft.VisualStudio.DesignTools.Markup.Metadata;
    using Microsoft.VisualStudio.DesignTools.Designer.Utility;
    using Microsoft.VisualStudio.DesignTools.Utility;
    using WindowsRuntime::Windows.Media.Core;
    using ColorHelper = WindowsRuntime::Windows.UI.ColorHelper;
    using ColorType = WindowsRuntime::Windows.UI.Color;
    using CornerRadiusType = WindowsRuntime::Windows.UI.Xaml.CornerRadius;
    using DurationType = WindowsRuntime::Windows.UI.Xaml.Duration;
    using Float = System.Single;
    using Globalization = WindowsRuntime::Windows.Globalization;
    using GridLengthType = WindowsRuntime::Windows.UI.Xaml.GridLength;
    using GridUnitTypeType = WindowsRuntime::Windows.UI.Xaml.GridUnitType;
    using InputScopeNameType = WindowsRuntime::Windows.UI.Xaml.Input.InputScopeName;
    using InputScopeNameValueType = WindowsRuntime::Windows.UI.Xaml.Input.InputScopeNameValue;
    using InputScopeType = WindowsRuntime::Windows.UI.Xaml.Input.InputScope;
    using MatrixType = WindowsRuntime::Windows.UI.Xaml.Media.Matrix;
    using PlatformMetadata = Microsoft.VisualStudio.DesignTools.UwpDesigner.Metadata.UwpPlatformMetadata;
    using PointType = WindowsRuntime::Windows.Foundation.Point;
    using RectType = WindowsRuntime::Windows.Foundation.Rect;
    using ScrollBarVisibilityType = WindowsRuntime::Windows.UI.Xaml.Controls.ScrollBarVisibility;
    using SizeType = WindowsRuntime::Windows.Foundation.Size;
    using ThicknessType = WindowsRuntime::Windows.UI.Xaml.Thickness;
    using TimeSpan = System.TimeSpan;
    using Xaml = WindowsRuntime::Windows.UI.Xaml;
    using XamlReader = WindowsRuntime::Windows.UI.Xaml.Markup.XamlReader;
    using XamlRepeatBehavior = WindowsRuntime::Windows.UI.Xaml.Media.Animation.RepeatBehavior;
    using XamlRepeatBehaviorType = WindowsRuntime::Windows.UI.Xaml.Media.Animation.RepeatBehaviorType;
    using XamlText = WindowsRuntime::Windows.UI.Text;

    // The type converters in this file are functional within the context Blend uses them, but are
    // not designed for general purpose use.
    public abstract class ConvertToFromStringConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string stringValue = value as string;
            if (stringValue != null)
            {
                return this.ConvertFromStringInternal(stringValue, culture);
            }
            return base.ConvertFrom(context, culture, value);
        }

        public abstract object ConvertFromStringInternal(string value, IFormatProvider formatProvider);

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return (destinationType == typeof(string)) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                string stringValue = value as string;
                if (stringValue == null && value != null)
                {
                    stringValue = this.ConvertToStringInternal(value, culture);
                }

                if (stringValue != null)
                {
                    return stringValue;
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public virtual string ConvertToStringInternal(object value, IFormatProvider formatProvider)
        {
            return null;
        }

        protected double StringToDouble(string s, IFormatProvider formatProvider, bool supportAuto)
        {
            if (supportAuto)
            {
                s = s.ToLowerInvariant();
                if (s == "auto")
                {
                    return double.NaN;
                }
            }
            return Convert.ToDouble(s, formatProvider);
        }

        protected string DoubleToString(double d, IFormatProvider formatProvider, bool supportAuto)
        {
            if (supportAuto && double.IsNaN(d))
            {
                return "Auto";
            }
            return Convert.ToString(d, formatProvider);
        }

        protected static char[] GetNumericListSeparator(IFormatProvider provider)
        {
            char ch = ',';
            NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
            if ((instance.NumberDecimalSeparator.Length > 0) && (ch == instance.NumberDecimalSeparator[0]))
            {
                ch = ';';
            }
            return new char[] {ch, ' ', '\t'};
        }
    }

    public abstract class ConvertUsingXamlReaderConverter<T> : ConvertToFromStringConverter
    {
        protected abstract PropertyInfo KnownProperty { get; }
        protected abstract Type KnownType { get; }

        public override object ConvertFromStringInternal(string value, IFormatProvider provider)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("<");
            stringBuilder.Append(this.KnownType.Name);
            stringBuilder.Append(" xmlns='" + XmlNamespace.PresentationUri + "' ");
            stringBuilder.Append(this.KnownProperty.Name);
            stringBuilder.Append("='");
            stringBuilder.Append(value);

            // If this is a framework element, we need to ensure that no default styles are being applied(we'll set an empty style).  The one known problem is parsing TextBlock.FontFamily, where default styles
            // will override local values until the element is placed in the visual tree.  Expression 105198.
            if (typeof(Xaml.FrameworkElement).IsAssignableFrom(this.KnownType))
            {
                stringBuilder.Append("'><FrameworkElement.Style><Style TargetType='");
                stringBuilder.Append(this.KnownType.Name);
                stringBuilder.Append("' /></FrameworkElement.Style></");
                stringBuilder.Append(this.KnownType.Name);
                stringBuilder.Append(">");
            }
            else
            {
                stringBuilder.Append("' />");
            }

            object instance = XamlReader.Load(stringBuilder.ToString());
            return this.KnownProperty.GetValue(instance, null);
        }
    }

    public abstract class StaticClassConverter : ConvertToFromStringConverter
    {
        protected abstract Type StaticType { get; }

        public override object ConvertFromStringInternal(string value, IFormatProvider provider)
        {
            PropertyInfo pi = this.StaticType.GetProperty(value, BindingFlags.Public | BindingFlags.Static);
            object instance = pi.GetValue(null, null);

            return instance;
        }
    }

    public abstract class StaticClassConverterEx : ConvertToFromStringConverter
    {
        // Eventually we'll migrate from StaticClassConverter to StaticClassConverterEx
        protected abstract List<object> StandardValues { get; }
        protected abstract Type StaticType { get; }

        protected static List<object> GetStandardValuesInternal(Type collectionType, Type objectType)
        {
            // Get all public static cursor propertiers from Cursors
            List<object> collection = new List<object>(); 
            PropertyInfo[] properties = collectionType.GetProperties(BindingFlags.Static | BindingFlags.Public);
            foreach (PropertyInfo property in properties)
            {
                object instance = property.GetValue(null, null);
                if (instance != null && objectType.IsAssignableFrom(instance.GetType()))
                {
                    collection.Add(instance);
                }
            }

            collection.Sort(new Comparison<object>(FontWeightConverter.ObjectAsStringComparer));
            return collection;
        }

        private static int ObjectAsStringComparer(object o1, object o2)
        {
            return o1.ToString().CompareTo(o2.ToString());
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return this.StandardValues.Count > 0;
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(this.StandardValues);
        }

        public override object ConvertFromStringInternal(string value, IFormatProvider provider)
        {
            if (value == null)
            {
                return null;
            }

            foreach (object item in this.StandardValues)
            {
                if (value.Equals(item.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
            }

            throw new InvalidOperationException(string.Format(ExceptionStringTable.StaticTypeConverterException, value, this.StaticType.Name));
        }
    }

#region Static class types

    public class FontWeightConverter : StaticClassConverterEx
    {
        private static readonly List<object> fontWeights = StaticClassConverterEx.GetStandardValuesInternal(typeof(XamlText.FontWeights), typeof(XamlText.FontWeight));
        protected override List<object> StandardValues { get { return FontWeightConverter.fontWeights; } }
        protected override Type StaticType { get { return typeof(XamlText.FontWeight); } }

        public override object ConvertFromStringInternal(string value, IFormatProvider provider)
        {
            if (value == null)
            {
                return null;
            }

            foreach (object item in this.StandardValues)
            {
                if (value.Equals(WindowsUIXamlFontWeightToString((XamlText.FontWeight) item), StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
            }

            throw new InvalidOperationException(string.Format(ExceptionStringTable.StaticTypeConverterException, value, this.StaticType.Name));
        }

        public override string ConvertToStringInternal(object value, IFormatProvider formatProvider)
        {
            if (value is XamlText.FontWeight)
            {
                return WindowsUIXamlFontWeightToString((XamlText.FontWeight)value);
            }
            else
            {
                return string.Empty;
            }
        }

        private static string WindowsUIXamlFontWeightToString(XamlText.FontWeight weight)
        {
            string convertedValue = string.Empty;
            switch (weight.Weight)
            {
                case 300:
                    return convertedValue = "Light";

                case 350:
                    return convertedValue = "SemiLight";

                case 400:
                    return convertedValue = "Normal";

                case 500:
                    return convertedValue = "Medium";

                case 100:
                    return convertedValue = "Thin";

                case 200:
                    return convertedValue = "ExtraLight";

                case 600:
                    return convertedValue = "SemiBold";

                case 700:
                    return convertedValue = "Bold";

                case 800:
                    return convertedValue = "ExtraBold";

                case 900:
                    return convertedValue = "Black";

                case 950:
                    return convertedValue = "ExtraBlack";
            }
            return convertedValue;
        }
    }

    public class CalendarIdentifierConverter : StaticClassConverterEx
    {
        private static readonly List<object> calendarIdentifiers = StaticClassConverterEx.GetStandardValuesInternal(typeof(Globalization.CalendarIdentifiers), typeof(string));
        protected override List<object> StandardValues { get { return CalendarIdentifierConverter.calendarIdentifiers; } }
        protected override Type StaticType { get { return typeof(string); } }
    }

    public class ClockIdentifierConverter : StaticClassConverterEx
    {
        private static readonly List<object> clockIdentifiers = StaticClassConverterEx.GetStandardValuesInternal(typeof(Globalization.ClockIdentifiers), typeof(string));
        protected override List<object> StandardValues { get { return ClockIdentifierConverter.clockIdentifiers; } }
        protected override Type StaticType { get { return typeof(string); } }
    }
#endregion

#region Hardcoded converters
    public class IMediaPlaybackSourceConverter : ConvertToFromStringConverter
    {
        public override object ConvertFromStringInternal(string value, IFormatProvider formatProvider)
        {
            return MediaSource.CreateFromUri(new Uri(value));
        }
    }

    public class ColorConverter : ConvertToFromStringConverter
    {
        public override object ConvertFromStringInternal(string value, IFormatProvider provider)
        {
            if (value.Length == 9 && value.StartsWith("#", StringComparison.Ordinal))
            {
                // Try to handle the most common case here to improve perf
                uint c = uint.Parse(value.Substring(1), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                return ColorHelper.FromArgb((byte)(c >> 24), (byte)(c >> 16), (byte)(c >> 8), (byte)c);
            }

            string xmlns = XmlNamespace.PresentationUri;
            Xaml.Media.SolidColorBrush brush = (Xaml.Media.SolidColorBrush)XamlReader.Load("<SolidColorBrush xmlns='" + xmlns + "' Color='" + value + "'/>");

            return brush.Color;
        }

        //Functions for serializing color names.
        internal static string ConvertColor(ColorType color, CultureInfo culture)
        {
            uint colorValue = (uint)(color.A << 24 | color.R << 16 | color.G << 8 | color.B);
            if (ColorConverter.StockColors.ContainsKey(colorValue))
            {
                return ColorConverter.StockColors[colorValue];
            }

            return string.Format(culture, "#{0:X2}{1:X2}{2:X2}{3:X2}", color.A, color.R, color.G, color.B);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string valueStr = value as string;
            if (valueStr != null)
                if (valueStr.Equals("Fuchsia", StringComparison.InvariantCultureIgnoreCase))
                    return Fuchsia;
                else if (valueStr.Equals("Aqua", StringComparison.InvariantCultureIgnoreCase))
                    return Aqua;
            return base.ConvertFrom(context, culture, value);
        }

        // Since Magenta and Fuchsia and Cyan and Aqua have the same color values, 
        // we create a slightly off color version to allow them to show up in the
        // drop-down list. This also allows us to preserve the user choice of name.
        // This doens't affect runtime (unless someone really wanted the 01 versions)
        // since the runtime will convert these values to the correct, non-off-color
        // versions.
        internal static ColorType Fuchsia = (ColorType)ColorHelper.FromArgb(0xFF, 0xFF, 0x01, 0xFF);
        internal static ColorType Aqua = (ColorType)ColorHelper.FromArgb(0xFF, 0x01, 0xFF, 0xFF);

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value is ColorType)
                {
                    ColorType color = (ColorType)value;
                    string result = ConvertColor(color, culture);
                    if (result != null)
                        return result;
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new TypeConverter.StandardValuesCollection(GetStockColorNames());
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        protected internal static System.Collections.ICollection GetStockColorNames()
        {
            return ColorConverter.StockColors.Values;
        }

#region Color Dictionary
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
#endregion
    }

    public class KeySplineConverter : ConvertUsingXamlReaderConverter<Xaml.Media.Animation.KeySpline>
    {
        private static Type splineDoubleKeyFrame;
        private static PropertyInfo keySpline;

        protected override PropertyInfo KnownProperty
        {
            get
            {
                if (keySpline == null)
                {
                    keySpline = PlatformMetadata.GetPropertyInfo(splineDoubleKeyFrame, "KeySpline");
                }
                return keySpline;
            }
        }

        protected override Type KnownType
        {
            get
            {
                if (splineDoubleKeyFrame == null)
                {
                    splineDoubleKeyFrame = typeof(Xaml.Media.Animation.SplineDoubleKeyFrame);
                }
                return splineDoubleKeyFrame;
            }
        }

        public override object ConvertFromStringInternal(string value, IFormatProvider provider)
        {
            Xaml.Media.Animation.KeySpline keySpline = base.ConvertFromStringInternal(value, provider) as Xaml.Media.Animation.KeySpline;
            if (keySpline == null)
            {
                return null;
            }

            // SL bug. KeySpline returned from XAML parsing will cause exceptions thrown when
            // we try to set it as a property of another object. 
            // WORKAROUND: create a new "fresh" KeySpline
            Xaml.Media.Animation.KeySpline result = new Xaml.Media.Animation.KeySpline();
            result.ControlPoint1 = keySpline.ControlPoint1;
            result.ControlPoint2 = keySpline.ControlPoint2;

            return result;
        }

        public override string ConvertToStringInternal(object value, IFormatProvider provider)
        {
            Xaml.Media.Animation.KeySpline spline = value as Xaml.Media.Animation.KeySpline;
            if (spline != null)
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}{4}{1}{4}{2}{4}{3}", 
                    new object[] {
                        RoundingHelper.RoundToDoublePrecision(spline.ControlPoint1.X, 6),
                        RoundingHelper.RoundToDoublePrecision(spline.ControlPoint1.Y, 6),
                        RoundingHelper.RoundToDoublePrecision(spline.ControlPoint2.X, 6), 
                        RoundingHelper.RoundToDoublePrecision(spline.ControlPoint2.Y, 6),
                        CultureInfo.InvariantCulture.TextInfo.ListSeparator
                    });
            }
            return base.ConvertToStringInternal(value, provider);
        }
    }

    public class KeyTimeConverter : ConvertToFromStringConverter
    {
        public override object ConvertFromStringInternal(string value, IFormatProvider provider)
        {
            return WindowsRuntime::Windows.UI.Xaml.Media.Animation.KeyTime.FromTimeSpan(TimeSpan.Parse(value));
        }
    }

    public class MatrixConverter : ConvertUsingXamlReaderConverter<MatrixType>
    {
        private static Type matrixTransform;
        private static PropertyInfo matrix;

        protected override PropertyInfo KnownProperty
        {
            get
            {
                if (matrix == null)
                {
                    matrix = PlatformMetadata.GetPropertyInfo(matrixTransform, "Matrix");
                }
                return matrix;
            }
        }

        protected override Type KnownType
        {
            get
            {
                if (matrixTransform == null)
                {
                    matrixTransform = typeof(Xaml.Media.MatrixTransform);
                }
                return matrixTransform;
            }
        }

        public override string ConvertToStringInternal(object value, IFormatProvider formatProvider)
        {
            if (value is MatrixType)
            {
                MatrixType matrix = (MatrixType)value;
                formatProvider = CultureInfo.GetCultureInfo("en-us");
                char separator = GetNumericListSeparator(formatProvider)[0];

                if (matrix.IsIdentity)
                {
                    return "Identity";
                }
                else
                {
                    return string.Format(formatProvider, "{0}{6}{1}{6}{2}{6}{3}{6}{4}{6}{5}", matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.OffsetX, matrix.OffsetY, separator);
                }
            }

            return base.ConvertToStringInternal(value, formatProvider);
        }
    }

    public class Matrix3DConverter : ConvertUsingXamlReaderConverter<Matrix3D>
    {
        private static Type matrix3DProjection;
        private static PropertyInfo projectionMatrix;

        protected override PropertyInfo KnownProperty
        {
            get
            {
                if (projectionMatrix == null)
                {
                    projectionMatrix = PlatformMetadata.GetPropertyInfo(matrix3DProjection, "ProjectionMatrix");
                }
                return projectionMatrix;
            }
        }

        protected override Type KnownType
        {
            get
            {
                if (matrix3DProjection == null)
                {
                    matrix3DProjection = typeof(Xaml.Media.Matrix3DProjection);
                }
                return matrix3DProjection;
            }
        }
    }

    public class PointConverter : ConvertToFromStringConverter
    {
        private static char[] numberSeparator = new char[] { ',', ' ' };
        public override object ConvertFromStringInternal(string value, IFormatProvider formatProvider)
        {
            // WPF uses en-US for Point
            IFormatProvider cultureInfo = CultureInfo.GetCultureInfo("en-us");
            value = value.Trim();
            string[] tokens = value.Split(GetNumericListSeparator(cultureInfo), StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 2)
            {
                return new PointType() { X = Float.Parse(tokens[0], cultureInfo), Y = Float.Parse(tokens[1], cultureInfo) };
            }
            throw new ArgumentException(string.Format(ExceptionStringTable.StaticTypeConverterException, value, typeof(PointType).Name));
        }

        public override string ConvertToStringInternal(object value, IFormatProvider formatProvider)
        {
            if(value == null)
            {
                throw new ArgumentException("value");
            }
            if (!(value is PointType))
            {
                throw new ArgumentException("value");
            }
            // WPF uses en-US for Point
            formatProvider = CultureInfo.GetCultureInfo("en-us");
            PointType point = (PointType)value;
            char separator = GetNumericListSeparator(formatProvider)[0];
            return string.Format("{1}{0}{2}", 
                                    separator,
                                    Convert.ToString(RoundingHelper.RoundToDoublePrecision(point.X, 6), formatProvider), 
                                    Convert.ToString(RoundingHelper.RoundToDoublePrecision(point.Y, 6), formatProvider));
        }
    }

    public class ThicknessConverter : ConvertToFromStringConverter
    {
        public override object ConvertFromStringInternal(string value, IFormatProvider formatProvider)
        {
            value = value.Trim();
            string[] tokens = value.Split(GetNumericListSeparator(formatProvider), StringSplitOptions.RemoveEmptyEntries);
            double[] values = new double[4];
            int index = 0;
            while (index < tokens.Length)
            {
                if (index >= 4)
                {
                    index = 5;
                    break;
                }
                values[index] = this.StringToDouble(tokens[index], formatProvider, true /* supportAuto */);
                index++;
            }
            switch (index)
            {
                case 1:
                    return new ThicknessType(values[0]);

                case 2:
                    return new ThicknessType(values[0], values[1], values[0], values[1]);

                case 4:
                    return new ThicknessType(values[0], values[1], values[2], values[3]);
            }
            throw new FormatException(string.Format(ExceptionStringTable.StaticTypeConverterException, value, typeof(ThicknessType).Name));
        }

        public override string ConvertToStringInternal(object value, IFormatProvider formatProvider)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (!(value is ThicknessType))
            {
                throw new ArgumentException("value");
            }
            ThicknessType thickness = (ThicknessType)value;
            char numericListSeparator = GetNumericListSeparator(formatProvider)[0];
            StringBuilder builder = new StringBuilder();
            if (thickness.Left == thickness.Right && thickness.Top == thickness.Bottom)
            {
                // At least N,M
                builder.Append(this.DoubleToString(thickness.Left, formatProvider, false /* supportAuto */));
                if (thickness.Left == thickness.Top)
                    // Only a N
                    return builder.ToString();
                else
                {
                    // N,M
                    builder.Append(numericListSeparator);
                    builder.Append(this.DoubleToString(thickness.Top, formatProvider, false /* supportAuto */));
                    return builder.ToString();
                }
            }
            builder.Append(this.DoubleToString(thickness.Left, formatProvider, false /* supportAuto */));
            builder.Append(numericListSeparator);
            builder.Append(this.DoubleToString(thickness.Top, formatProvider, false /* supportAuto */));
            builder.Append(numericListSeparator);
            builder.Append(this.DoubleToString(thickness.Right, formatProvider, false /* supportAuto */));
            builder.Append(numericListSeparator);
            builder.Append(this.DoubleToString(thickness.Bottom, formatProvider, false /* supportAuto */));
            return builder.ToString();
        }
    }

    public class NullableBoolConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string) || sourceType == typeof(bool))
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

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is bool)
            {
                return (bool?)value;
            }

            string str = value as string;
            if (!string.IsNullOrEmpty(str))
            {
                return bool.Parse(str);
            }
            else if (value == null)
            {
                return null;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value is bool)
                {
                    bool boolValue = (bool)value;
                    string result = boolValue.ToString();
                    if (result != null)
                        return result;
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class ScrollBarVisibilityConverter : TypeConverter
    {
        private StandardValuesCollection scrollBarVisibilityOptions;
    
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if ((sourceType == typeof(string)) || (sourceType == typeof(ScrollBarVisibilityType)))
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

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is ScrollBarVisibilityType)
            {
                return (ScrollBarVisibilityType?)value;
            }

            string stringValue = value as string;

            if (!string.IsNullOrEmpty(stringValue))
            {
                return ScrollBarVisibilityConverter.ConvertFromStringInternal(stringValue, culture);
            }
            else if (value == null)
            {
                return null;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if ((destinationType == typeof(string)) && (value is ScrollBarVisibilityType))
            {
                return ScrollBarVisibilityConverter.ConvertToStringInternal(value, culture);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        private static object ConvertFromStringInternal(string value, IFormatProvider formatProvider)
        {
            ScrollBarVisibilityType retVal;

            if (value.Equals("auto", StringComparison.InvariantCultureIgnoreCase))
            {
                retVal = ScrollBarVisibilityType.Auto;
            }
            else if (value.Equals("disabled", StringComparison.InvariantCultureIgnoreCase))
            {
                retVal = ScrollBarVisibilityType.Disabled;
            }
            else if (value.Equals("hidden", StringComparison.InvariantCultureIgnoreCase))
            {
                retVal = ScrollBarVisibilityType.Hidden;
            }
            else if (value.Equals("visible", StringComparison.InvariantCultureIgnoreCase))
            {
                retVal = ScrollBarVisibilityType.Visible;
            }
            else
            {
                return null;
            }

            return retVal;
        }

        private static string ConvertToStringInternal(object value, IFormatProvider formatProvider)
        {
            switch ((ScrollBarVisibilityType)value)
            {
                case ScrollBarVisibilityType.Auto:
                    return "Auto";
                case ScrollBarVisibilityType.Disabled:
                    return "Disabled";
                case ScrollBarVisibilityType.Hidden:
                    return "Hidden";
                case ScrollBarVisibilityType.Visible:
                    return "Visible";
                default:
                    return string.Empty;
            }
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (this.scrollBarVisibilityOptions == null)
            {
                this.scrollBarVisibilityOptions = 
                    new StandardValuesCollection(ScrollBarVisibilityConverter.GetVisibilityValues(context));
            }

            return this.scrollBarVisibilityOptions;
        }

        private static ScrollBarVisibilityType[] GetVisibilityValues(ITypeDescriptorContext context)
        {
            return new ScrollBarVisibilityType[] {
                ScrollBarVisibilityType.Auto,
                ScrollBarVisibilityType.Disabled,
                ScrollBarVisibilityType.Hidden,
                ScrollBarVisibilityType.Visible
            };
        }
    }

    public abstract class StringCollectionConverter : ConvertToFromStringConverter
    {
        protected abstract StandardValuesCollection StandardValues { get; }

        public override object ConvertFromStringInternal(string value, IFormatProvider formatProvider)
        {
            return value;
        }

        public override string ConvertToStringInternal(object value, IFormatProvider formatProvider)
        {
            string valueString = value as string;
            return valueString;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return this.StandardValues;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }
    }

    public class DayFormatConverter : StringCollectionConverter
    {
        private static readonly StandardValuesCollection commonDayFormats = 
            new StandardValuesCollection(new string[] { 
                "{day.integer}",
                "{day.integer(2)}",
                "{dayofweek.full}",
                "{dayofweek.abbreviated}",
                "{dayofweek.abbreviated(3)}",
                "{dayofweek.solo.full}",
                "{dayofweek.solo.abbreviated}",
                "{dayofweek.solo.abbreviated(3)}",
                "{day.integer} {dayofweek.full}",
                "{day.integer} {dayofweek.abbreviated}"
            });

        protected override StandardValuesCollection StandardValues
        {
            get { return DayFormatConverter.commonDayFormats; }
        }
    }

    public class MonthFormatConverter : StringCollectionConverter
    {
        private static readonly StandardValuesCollection commonMonthFormats =
            new StandardValuesCollection(new string[] { 
                "{month.integer}",
                "{month.integer(2)}",
                "{month.full}",
                "{month.abbreviated}",
                "{month.abbreviated(3)}",
                "{month.solo.full}",
                "{month.solo.abbreviated}",
                "{month.solo.abbreviated(3)}",
                "{month.integer} {month.full}",
                "{month.integer} {month.abbreviated}"
            });

        protected override StandardValuesCollection StandardValues
        {
            get { return MonthFormatConverter.commonMonthFormats; }
        }
    }

    public class YearFormatConverter : StringCollectionConverter
    {
        private static readonly StandardValuesCollection commonYearFormats =
            new StandardValuesCollection(new string[] { 
                "{year.full}",
                "{year.full(2)}",
                "{year.full(4)}",
                "{year.abbreviated}",
                "{year.abbreviated(2)}",
                "{year.abbreviated(4)}"
            });

        protected override StandardValuesCollection StandardValues
        {
            get { return YearFormatConverter.commonYearFormats; }
        }
    }

    public class DateFormatConverter : StringCollectionConverter
    {
        private static readonly StandardValuesCollection commonDateFormats =
            new StandardValuesCollection(new string[] { 
                "{day.integer}/{month.integer}/{year.full}",
                "{day.integer} {month.full} {year.full}",
                "{month.integer}/{day.integer}/{year.full}",
                "{month.full} {day.integer}, {year.full}",
                "{year.full}/{month.integer}/{day.integer}"
            });

        protected override StandardValuesCollection StandardValues
        {
            get { return DateFormatConverter.commonDateFormats; }
        }
    }

        public class DayOfWeekFormatConverter : StringCollectionConverter
    {
        private static readonly StandardValuesCollection commonDayOfWeekFormats =
            new StandardValuesCollection(new string[] { 
                "{dayofweek.abbreviated}",
                "{dayofweek.abbreviated(2)}",
                "{dayofweek.abbreviated(3)}",
                "{dayofweek.solo.abbreviated}",
                "{dayofweek.solo.abbreviated(2)}",
                "{dayofweek.solo.abbreviated(3)}"
            });

        protected override StandardValuesCollection StandardValues
        {
            get { return DayOfWeekFormatConverter.commonDayOfWeekFormats; }
        }
    }

    public class SymbolIconSymbolConverter : EnumConverter
    {
        public SymbolIconSymbolConverter()
            : base(typeof(Xaml.Controls.Symbol))
        { }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return SymbolIconSymbolConverter.GetSymbolEnumValues();
        }

        internal static StandardValuesCollection GetSymbolEnumValues()
        {
            Array symbols = Enum.GetValues(typeof(Xaml.Controls.Symbol));
            Array.Sort(symbols, new SymbolComparer());
            return new TypeConverter.StandardValuesCollection(symbols);
        }

        private class SymbolComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                return StringComparer.InvariantCulture.Compare(x.ToString(), y.ToString());
            }
        }
    }

    public class SymbolIconConverter : ConvertToFromStringConverter
    {
        public override object ConvertFromStringInternal(string value, IFormatProvider formatProvider)
        {
            Xaml.Controls.Symbol symbol;
            
            if (Enum.TryParse<Xaml.Controls.Symbol>(value, out symbol))
            {
                return new Xaml.Controls.SymbolIcon(symbol);
            }

            throw new FormatException(string.Format(ExceptionStringTable.StaticTypeConverterException, value, typeof(Xaml.Controls.SymbolIcon).Name));
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return SymbolIconSymbolConverter.GetSymbolEnumValues();
        }
    }

    public class FontSizeConverter : TypeConverter
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
            double tryDouble;
            string stringValue = value as string;
            while (!string.IsNullOrEmpty(stringValue))
            {
                if (double.TryParse(stringValue, NumberStyles.Float | NumberStyles.AllowThousands, culture, out tryDouble))
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

    public class CornerRadiusConverter : ConvertToFromStringConverter
    {
        public override object ConvertFromStringInternal(string value, IFormatProvider provider)
        {
            value = value.Trim();
            string[] tokens = value.Split(GetNumericListSeparator(provider), StringSplitOptions.RemoveEmptyEntries);
            double[] values = new double[4];
            int i = 0;
            while (i < tokens.Length)
            {
                if (i >= 4)
                {
                    i = 5;
                    break;
                }
                values[i] = double.Parse(tokens[i], provider);
                i++;
            }
            if (i == 1)
            {
                //<object property="AllCorners"/>
                return new CornerRadiusType() { TopLeft=values[0], TopRight=values[0], BottomRight=values[0], BottomLeft=values[0] };
            }
            else if (i == 4)
            {
                //<object property="TopLeft,TopRight,BottomRight,BottomLeft "/>
                return new CornerRadiusType() { TopLeft=values[0], TopRight=values[1], BottomRight=values[2], BottomLeft=values[3] };
            }
            throw new FormatException(string.Format(ExceptionStringTable.StaticTypeConverterException, value, typeof(CornerRadiusType).Name));
        }

        public override string ConvertToStringInternal(object value, IFormatProvider formatProvider)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (!(value is CornerRadiusType))
            {
                throw new ArgumentException("value");
            }
            CornerRadiusType cr = (CornerRadiusType)value;
            char numericListSeparator = GetNumericListSeparator(formatProvider)[0];
            StringBuilder builder = new StringBuilder();
            if (cr.TopLeft == cr.TopRight && cr.TopRight == cr.BottomLeft && cr.BottomLeft == cr.BottomRight)
            {
                //Serialize CornerRadius(N,N,N,N) to "N"
                builder.Append(this.DoubleToString(cr.TopLeft, formatProvider, false /* supportAuto */));
                return builder.ToString();
            } 
            builder.Append(this.DoubleToString(cr.TopLeft, formatProvider, false /* supportAuto */));
            builder.Append(numericListSeparator);
            builder.Append(this.DoubleToString(cr.TopRight, formatProvider, false /* supportAuto */));
            builder.Append(numericListSeparator);
            builder.Append(this.DoubleToString(cr.BottomRight, formatProvider, false /* supportAuto */));
            builder.Append(numericListSeparator);
            builder.Append(this.DoubleToString(cr.BottomLeft, formatProvider, false /* supportAuto */));
            return builder.ToString();
        }
    }

    public class RectConverter : ConvertUsingXamlReaderConverter<RectType>
    {
        private static Type rectangleGeometry;
        private static PropertyInfo rect;
        
        protected override PropertyInfo KnownProperty
        {
            get
            {
                if (rect == null)
                {
                    rect = PlatformMetadata.GetPropertyInfo(rectangleGeometry, "Rect");
                }
                return rect;
            }
        }

        protected override Type KnownType
        {
            get
            {
                if (rectangleGeometry == null)
                {
                    rectangleGeometry = typeof(Xaml.Media.RectangleGeometry);
                }
                return rectangleGeometry;
            }
        }

        public override string ConvertToStringInternal(object value, IFormatProvider provider)
        {
            RectType? rect = value as RectType?;
            if (rect != null)
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}{4}{1}{4}{2}{4}{3}", 
                    new object[] { 
                        RoundingHelper.RoundToDoublePrecision(rect.Value.X, 6),
                        RoundingHelper.RoundToDoublePrecision(rect.Value.Y, 6),
                        RoundingHelper.RoundToDoublePrecision(rect.Value.Width, 6), 
                        RoundingHelper.RoundToDoublePrecision(rect.Value.Height, 6),
                        CultureInfo.InvariantCulture.TextInfo.ListSeparator 
                    });
            }
            return base.ConvertToStringInternal(value, provider);
        }
    }

    public class SizeConverter : ConvertUsingXamlReaderConverter<SizeType>
    {
        private static Type arcSegment;
        private static PropertyInfo size;

        protected override PropertyInfo KnownProperty
        {
            get
            {
                if (size == null)
                {
                    size = PlatformMetadata.GetPropertyInfo(arcSegment, "Size");
                }
                return size;
            }
        }

        protected override Type KnownType
        {
            get
            {
                if (arcSegment == null)
                {
                    arcSegment = typeof(Xaml.Media.ArcSegment);
                }
                return arcSegment;
            }
        }

        public override string ConvertToStringInternal(object value, IFormatProvider formatProvider)
        {
            if (value is SizeType)
            {
                formatProvider = CultureInfo.GetCultureInfo("en-us");
                char separator = GetNumericListSeparator(formatProvider)[0];

                SizeType size = (SizeType)value;
                return string.Format(formatProvider, "{0}{2}{1}", RoundingHelper.RoundToDoublePrecision(size.Width, 6), RoundingHelper.RoundToDoublePrecision(size.Height, 6), separator);
            }

            return base.ConvertToStringInternal(value, formatProvider);
        }
    }

    public class ImageSourceConverter : TypeConverter
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

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                throw base.GetConvertFromException(value);
            }

            Uri imageUri = value as Uri;
            if (imageUri == null)
            {
                string imageUriString = value as string;
                if (!String.IsNullOrEmpty(imageUriString))
                {
                    Uri.TryCreate(imageUriString, UriKind.RelativeOrAbsolute, out imageUri);
                }
            }

            if (imageUri != null)
            {
                return new Xaml.Media.Imaging.BitmapImage(imageUri);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if ((destinationType != null) && (value is Xaml.Media.ImageSource))
            {
                if (destinationType == typeof(string))
                {
                    Xaml.Media.Imaging.BitmapImage image = value as Xaml.Media.Imaging.BitmapImage;
                    if (image != null)
                    {
                        return image.UriSource.OriginalString;
                    }

                    return null;
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class InputScopeConverter : StaticClassConverterEx
    {
        protected override Type StaticType 
        { 
            get 
            { 
                return typeof(Xaml.Input.InputScope); 
            } 
        }

        private List<object> standardValues;

        protected override List<object> StandardValues
        {
            get
            {
                if (this.standardValues == null)
                {
                    this.standardValues = new List<object>();
                    Type referenceType = WindowsRuntimeService.GetReferenceType(typeof(InputScopeNameValueType).FullName) ?? typeof(InputScopeNameValueType);
                    List<string> names = referenceType?.GetEnumNames().ToList() ?? new List<string>();
                    foreach (string name in names)
                    {
                        InputScopeNameValueType nameValue;
                        InputScopeType inputScope = new InputScopeType();
                        if (Enum.TryParse(name, out nameValue))
                        {
                            inputScope.Names.Add(new InputScopeNameType(nameValue));
                            this.standardValues.Add(inputScope);
                        }
                    }
                }
                return this.standardValues;
            }
        }

        public override object ConvertFromStringInternal(string value, IFormatProvider provider)
        {
            string valueStr = value as string;
            if (valueStr != null)
            {
                InputScopeType inputScope = new InputScopeType();
                foreach (string name in valueStr.Split(' '))
                {
                    InputScopeNameValueType nameValue;
                    if (Enum.TryParse(name, out nameValue))
                    {
                        inputScope.Names.Add(new InputScopeNameType(nameValue));
                    }
                    else
                    {
                        throw new InvalidOperationException(string.Format(ExceptionStringTable.StaticTypeConverterException, value, typeof(InputScopeType).Name));
                    }
                }
                return inputScope;
            }

            throw new InvalidOperationException(string.Format(ExceptionStringTable.StaticTypeConverterException, value, this.StaticType.Name));
        }

        public override string ConvertToStringInternal(object value, IFormatProvider formatProvider)
        {
            InputScopeType inputScope = value as InputScopeType;
            if (inputScope != null)
            {
                List<string> names = new List<string>();
                foreach (InputScopeNameType inputScopeName in inputScope.Names)
                {
                    names.Add(inputScopeName.NameValue.ToString());
                }
                return String.Join(" ", names);
            }
            return null;
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new TypeConverter.StandardValuesCollection(this.StandardValues);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }

    // Not unit-tested
    public class TemplateKeyConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            throw new NotSupportedException();
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new NotSupportedException();
        }
    }

    public class GridLengthConverter : ConvertToFromStringConverter
    {
        public override object ConvertFromStringInternal(string value, IFormatProvider provider)
        {
            value = value.Trim();
            if (string.Compare(value, "auto", true, CultureInfo.InvariantCulture) == 0)
            {
                return XamlExtensions.CreateGridLength(0, GridUnitTypeType.Auto);
            }

            GridUnitTypeType unitType;

            if (value[value.Length - 1] == '*')
            {
                unitType = GridUnitTypeType.Star;

                if (value == "*")
                {
                    return XamlExtensions.CreateGridLength(1, unitType);
                }

                value = value.Substring(0, value.Length - 1);
            }
            else
            {
                unitType = GridUnitTypeType.Pixel;
            }
            return XamlExtensions.CreateGridLength(double.Parse(value, provider), unitType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            GridLengthType gridLength = (GridLengthType)value;

            if (gridLength.GridUnitType == GridUnitTypeType.Auto)
            {
                return "Auto";
            }
            else if (gridLength.GridUnitType == GridUnitTypeType.Star)
            {
                if (gridLength.Value == 1.0)
                {
                    return "*";
                }
                else
                {
                    return gridLength.Value.ToString(CultureInfo.InvariantCulture) + "*";
                }
            }
            else
            {
                return gridLength.Value.ToString(CultureInfo.InvariantCulture);
            }
        }
    }

#endregion

#region ConvertUsingXamlReader converters
    public class BrushConverter : ConvertUsingXamlReaderConverter<Xaml.Media.Brush>
    {
        private static Type canvas;
        private static PropertyInfo background;

        protected override PropertyInfo KnownProperty
        {
            get
            {
                if (background == null)
                {
                    background = PlatformMetadata.GetPropertyInfo(canvas, "Background");
                }
                return background;
            }
        }

        protected override Type KnownType
        {
            get
            {
                if (canvas == null)
                {
                    canvas = typeof(Xaml.Controls.Canvas);
                }
                return canvas;
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            Xaml.Media.SolidColorBrush solidColorBrush = value as Xaml.Media.SolidColorBrush;
            if (solidColorBrush != null)
            {
                //convert from a solid color string to brush 
                string color = colorConverter.ConvertTo(context, culture, solidColorBrush.Color, destinationType) as string;
                if (color != null)
                    return color;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
        
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string valueStr = value as string;
            if (valueStr != null)
                if (valueStr.Equals("Fuchsia", StringComparison.InvariantCultureIgnoreCase))
                    return new Xaml.Media.SolidColorBrush() { Color = ColorConverter.Fuchsia };
                else if (valueStr.Equals("Aqua", StringComparison.InvariantCultureIgnoreCase))
                    return new Xaml.Media.SolidColorBrush() { Color = ColorConverter.Aqua };
            return base.ConvertFrom(context, culture, value);
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new TypeConverter.StandardValuesCollection(GetStockBrushNames());
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        private System.Collections.ICollection GetStockBrushNames()
        {
            return ColorConverter.GetStockColorNames();
        }
        //converter used in converting color to color names and hex values
        private static TypeConverter colorConverter = new ColorConverter();

    }

    public sealed class CacheModeConverter : ConvertUsingXamlReaderConverter<Xaml.Media.CacheMode>
    {
        private static Type uiElement;
        private static PropertyInfo cacheMode;

        protected override PropertyInfo KnownProperty
        {
            get
            {
                if (cacheMode == null)
                {
                    cacheMode = PlatformMetadata.GetPropertyInfo(uiElement, "CacheMode");
                }
                return cacheMode;
            }
        }

        protected override Type KnownType
        {
            get
            {
                if (uiElement == null)
                {
                    uiElement = typeof(Xaml.Controls.Canvas);
                }
                return uiElement;
            }
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(value, attributes);
        }

        public override string ConvertToStringInternal(object value, IFormatProvider formatProvider)
        {
            return value.GetType().Name;
        }
    }

    public class DurationConverter : ConvertUsingXamlReaderConverter<DurationType>
    {
        private static Type storyboard;
        private static PropertyInfo duration;

        protected override PropertyInfo KnownProperty
        {
            get
            {
                if (duration == null)
                {
                    duration = PlatformMetadata.GetPropertyInfo(storyboard, "Duration");
                }
                return duration;
            }
        }

        protected override Type KnownType
        {
            get
            {
                if (storyboard == null)
                {
                    storyboard = typeof(Xaml.Media.Animation.Storyboard);
                }
                return storyboard;
            }
        }

        public override object ConvertFromStringInternal(string value, IFormatProvider provider)
        {
            value = value.Trim();

            if (value == "Automatic")
            {
                return DurationType.Automatic;
            }
            else if (value == "Forever")
            {
                return DurationType.Forever;
            }

            TimeSpan timeSpan;
            if (string.IsNullOrEmpty(value) || !TimeSpan.TryParse(value, provider, out timeSpan))
            {
                timeSpan = new TimeSpan();
            }
            return new DurationType(timeSpan);
        }
    }

    public class FontFamilyConverter : ConvertUsingXamlReaderConverter<Xaml.Media.FontFamily>
    {
        private static Type textBlock;
        private static PropertyInfo fontFamily;

        protected override PropertyInfo KnownProperty
        {
            get
            {
                if (fontFamily == null)
                {
                    fontFamily = PlatformMetadata.GetPropertyInfo(textBlock, "FontFamily");
                }
                return fontFamily;
            }
        }

        protected override Type KnownType
        {
            get
            {
                if (textBlock == null)
                {
                    textBlock = typeof(Xaml.Controls.TextBlock);
                }
                return textBlock;
            }
        }

        public override string ConvertToStringInternal(object value, IFormatProvider formatProvider)
        {
            Xaml.Media.FontFamily fontFamily = value as Xaml.Media.FontFamily;
            if (fontFamily != null)
            {
                return string.Format(formatProvider, "{0}", fontFamily.Source);
            }

            return base.ConvertToStringInternal(value, formatProvider);
        }
    }

    // Not unit tested
    public class TransformConverter : ConvertUsingXamlReaderConverter<Xaml.Media.Transform>
    {
        private static Type brush;
        private static PropertyInfo transform;

        protected override PropertyInfo KnownProperty
        {
            get
            {
                if (transform == null)
                {
                    transform = PlatformMetadata.GetPropertyInfo(brush, "Transform");
                }
                return transform;
            }
        }

        protected override Type KnownType
        {
            get
            {
                if (brush == null)
                {
                    brush = typeof(Xaml.Media.Brush);
                }
                return brush;
            }
        }
    }

    // Not unit tested
    public class ProjectionConverter : ConvertUsingXamlReaderConverter<Xaml.Media.Projection>
    {
        private static Type brush;
        private static PropertyInfo projection;

        protected override PropertyInfo KnownProperty
        {
            get
            {
                if (projection == null)
                {
                    projection = PlatformMetadata.GetPropertyInfo(brush, "Projection");
                }
                return projection;
            }
        }

        protected override Type KnownType
        {
            get
            {
                if (brush == null)
                {
                    brush = typeof(Xaml.Media.Brush);
                }
                return brush;
            }
        }
    }

    public class RepeatBehaviorConverter : ConvertUsingXamlReaderConverter<XamlRepeatBehavior>
    {
        private static Type storyboard;
        private static PropertyInfo repeatBehavior;

        protected override PropertyInfo KnownProperty
        {
            get
            {
                if (repeatBehavior == null)
                {
                    repeatBehavior = PlatformMetadata.GetPropertyInfo(storyboard, "RepeatBehavior");
                }
                return repeatBehavior;
            }
        }

        protected override Type KnownType
        {
            get
            {
                if (storyboard == null)
                {
                    storyboard = typeof(Xaml.Media.Animation.Storyboard);
                }
                return storyboard;
            }
        }

        public override string ConvertToStringInternal(object value, IFormatProvider formatProvider)
        {
            if (value is XamlRepeatBehavior)
            {
                XamlRepeatBehavior repeatBehavior = (XamlRepeatBehavior)value;
                if (repeatBehavior.Count != 0)
                {
                    return string.Format(formatProvider, "{0}x", repeatBehavior.Count);
                }
                if (repeatBehavior.Type == XamlRepeatBehaviorType.Forever)
                {
                    return XamlRepeatBehaviorType.Forever.ToString();
                }
            }

            return base.ConvertToStringInternal(value, formatProvider);
        }
    }

    public class DoubleCollectionConverter : ConvertUsingXamlReaderConverter<Xaml.Media.DoubleCollection>
    {
        private static Type path;
        private static PropertyInfo strokeDashArray;

        protected override PropertyInfo KnownProperty
        {
            get
            {
                if (strokeDashArray == null)
                {
                    strokeDashArray = PlatformMetadata.GetPropertyInfo(path, "StrokeDashArray");
                }
                return strokeDashArray;
            }
        }

        protected override Type KnownType
        {
            get
            {
                if (path == null)
                {
                    path = typeof(Xaml.Shapes.Path);
                }
                return path;
            }
        }

        public override object ConvertFromStringInternal(string value, IFormatProvider provider)
        {
            Xaml.Media.DoubleCollection doubles = (Xaml.Media.DoubleCollection)base.ConvertFromStringInternal(value, provider);
            // We have to clone this value, else silverlight throws.
            Xaml.Media.DoubleCollection copy = new Xaml.Media.DoubleCollection();
            foreach (double doubleValue in doubles) copy.Add(doubleValue);
            return copy;
        }

        public override string ConvertToStringInternal(object value, IFormatProvider formatProvider)
        {
            Xaml.Media.DoubleCollection doubles = (Xaml.Media.DoubleCollection)value;
            if (doubles.Count() != 0)
            {
                StringBuilder result = new StringBuilder();
                result.Append(this.DoubleToString(doubles.GetItem(0), formatProvider, false));
                for (int i = 1; i < doubles.Count(); i++)
                {
                    result.Append(" ");
                    result.Append(this.DoubleToString(doubles.GetItem(i), formatProvider, false));
                }
                return result.ToString();
            }
            return string.Empty;
        }
    }

    public class PointCollectionConverter : ConvertUsingXamlReaderConverter<Xaml.Media.PointCollection>
    {
        private static Type polyline;
        private static PropertyInfo points;

        protected override PropertyInfo KnownProperty
        {
            get
            {
                if (points == null)
                {
                    points = PlatformMetadata.GetPropertyInfo(polyline, "Points");
                }
                return points;
            }
        }

        protected override Type KnownType
        {
            get
            {
                if (polyline == null)
                {
                    polyline = typeof(Xaml.Shapes.Polyline);
                }
                return polyline;
            }
        }

        public override object ConvertFromStringInternal(string value, IFormatProvider provider)
        {
            Xaml.Media.PointCollection points = (Xaml.Media.PointCollection)base.ConvertFromStringInternal(value, provider);
            // We have to clone this value, else silverlight throws.
            Xaml.Media.PointCollection copy = new Xaml.Media.PointCollection();
            foreach (PointType point in points)
            {
                copy.Add(point);
            }
            return copy;
        }
    }

    public class GeometryConverter : ConvertToFromStringConverter
    {
        public override object ConvertFromStringInternal(string value, IFormatProvider provider)
        {
            Xaml.Media.PathGeometry geometry = new Xaml.Media.PathGeometry();

            int index = 0;
            while ((index < value.Length) && char.IsWhiteSpace(value, index))
            {
                index++;
            }
            if ((index < value.Length) && (value[index] == 'F'))
            {
                index++;
                while ((index < value.Length) && char.IsWhiteSpace(value, index))
                {
                    index++;
                }
                if ((index == value.Length) || ((value[index] != '0') && (value[index] != '1')))
                {
                    throw new FormatException();
                }

                geometry.FillRule = (value[index] == '0') ? Xaml.Media.FillRule.EvenOdd : Xaml.Media.FillRule.Nonzero;
                index++;
            }

            new AbbreviatedGeometryParser(geometry).Parse(value, index);

            return geometry;
        }

        private class AbbreviatedGeometryParser
        {
            private Xaml.Media.PathGeometry geometry;

            private Xaml.Media.PathFigure figure;
            private PointType lastPoint;
            private PointType secondLastPoint;
            private PointType lastStart;

            private string buffer;
            private int index;
            private int length;
            private char token;

            public AbbreviatedGeometryParser(Xaml.Media.PathGeometry geometry)
            {
                this.geometry = geometry;
            }

            public void Parse(string data, int startIndex)
            {
                this.buffer = data;
                this.length = data.Length;
                this.index = startIndex;

                bool first = true;
                char ch = ' ';

                while (this.ReadToken())
                {
                    char command = this.token;

                    if (first)
                    {
                        if ((command != 'M') && (command != 'm'))
                        {
                            throw new FormatException();
                        }

                        first = false;
                    }

                    switch (command)
                    {
                        case 'M':
                        case 'm':
                            this.lastPoint = this.ReadPoint(command, false);
                            this.BeginFigure(this.lastPoint);
                            this.lastStart = this.lastPoint;
                            for (ch = 'M'; this.IsNumber(true); ch = 'L')
                            {
                                this.lastPoint = this.ReadPoint(ch, false);
                                this.LineTo(this.lastPoint);
                            }
                            continue;

                        case 'L':
                        case 'l':
                            this.EnsureFigure();
                            do
                            {
                                this.lastPoint = this.ReadPoint(command, false);
                                this.LineTo(this.lastPoint);
                            }
                            while (this.IsNumber(true));
                            break;

                        case 'H':
                        case 'h':
                            this.EnsureFigure();
                            do
                            {
                                Float x = (Float)this.ReadDouble(false);
                                if (command == 'h')
                                {
                                    x += (Float)this.lastPoint.X;
                                }
                                this.lastPoint.X = x;
                                this.LineTo(this.lastPoint);
                            }
                            while (this.IsNumber(true));
                            break;

                        case 'V':
                        case 'v':
                            this.EnsureFigure();
                            do
                            {
                                Float y = (Float)this.ReadDouble(false);
                                if (command == 'v')
                                {
                                    y += (Float)this.lastPoint.Y;
                                }
                                this.lastPoint.Y = y;
                                this.LineTo(this.lastPoint);
                            }
                            while (this.IsNumber(true));
                            break;

                        case 'C':
                        case 'c':
                            this.EnsureFigure();
                            do
                            {
                                PointType point = this.ReadPoint(command, false);
                                this.secondLastPoint = this.ReadPoint(command, true);
                                this.lastPoint = this.ReadPoint(command, true);
                                this.BezierTo(point, this.secondLastPoint, this.lastPoint);
                                ch = 'C';
                            }
                            while (this.IsNumber(true));
                            break;

                        case 'S':
                        case 's':
                            this.EnsureFigure();
                            do
                            {
                                PointType point1 = this.GetSmoothBeizerFirstPoint();
                                PointType point2 = this.ReadPoint(command, false);
                                this.lastPoint = this.ReadPoint(command, true);
                                this.BezierTo(point1, point2, this.lastPoint);
                                ch = 'S';
                            }
                            while (this.IsNumber(true));
                            break;

                        case 'Q':
                        case 'q':
                            this.EnsureFigure();
                            do
                            {
                                PointType point = this.ReadPoint(command, false);
                                this.lastPoint = this.ReadPoint(command, true);
                                this.QuadraticBezierTo(point, this.lastPoint);
                                ch = 'Q';
                            }
                            while (this.IsNumber(true));
                            break;

                        case 'A': // A Size.Width,Size.Height,angle,0|1(isLargeArc),0|1(sweepDirection),Point.X, Point.Y
                        case 'a':
                            {
                                do
                                {
                                    SizeType size = this.ReadSize(false);
                                    Float rotationAngle = this.ReadDouble(true);
                                    bool isLargeArc = this.ReadBool01(true);
                                    bool direction = this.ReadBool01(true);
                                    Xaml.Media.SweepDirection sweepDirection = (direction ? Xaml.Media.SweepDirection.Clockwise : Xaml.Media.SweepDirection.Counterclockwise);
                                    this.lastPoint = this.ReadPoint(command, true);
                                    this.ArcTo(size, rotationAngle, isLargeArc, sweepDirection, this.lastPoint);
                                    ch = 'A';
                                }
                                while (this.IsNumber(true));
                            }
                            this.EnsureFigure();
                            break;

                        case 'Z':
                        case 'z':
                            this.FinishFigure(true);
                            break;

                        default:
                            throw new NotSupportedException();
                    }
                }

                this.FinishFigure(false);
            }

            private bool ReadToken()
            {
                this.SkipWhitespace(false);
                if (this.index < this.length)
                {
                    this.token = this.buffer[this.index++];
                    return true;
                }

                return false;
            }

            private PointType ReadPoint(char command, bool allowComma)
            {
                Float x = this.ReadDouble(allowComma);
                Float y = this.ReadDouble(true);
                if (command >= 'a')
                {
                    x += (Float)this.lastPoint.X;
                    y += (Float)this.lastPoint.Y;
                }

                return new PointType() { X = x, Y = y };
            }

            private SizeType ReadSize(bool allowComma)
            {
                Float width = this.ReadDouble(allowComma);
                Float height = this.ReadDouble(true);
                return new SizeType() { Width = width, Height = height };
            }

            private bool ReadBool01(bool allowComma)
            {
                // We expect to see either 0 (false) or 1 (true). Anything else is not allowed
                Float d = this.ReadDouble(allowComma);
                if (d == 0)
                {
                    return false;
                }
                else if (d == 1)
                {
                    return true;
                }
                throw new FormatException();
            }

            private Float ReadDouble(bool allowComma)
            {
                double num4;
                if (!this.IsNumber(allowComma))
                {
                    throw new FormatException();
                }
                bool flag = true;
                int startIndex = this.index;
                if ((this.index < this.length) && ((this.buffer[this.index] == '-') || (this.buffer[this.index] == '+')))
                {
                    this.index++;
                }
                if ((this.index < this.length) && (this.buffer[this.index] == 'I'))
                {
                    this.index = Math.Min(this.index + 8, this.length);
                    flag = false;
                }
                else if ((this.index < this.length) && (this.buffer[this.index] == 'N'))
                {
                    this.index = Math.Min(this.index + 3, this.length);
                    flag = false;
                }
                else
                {
                    this.SkipDigits(false);
                    if ((this.index < this.length) && (this.buffer[this.index] == '.'))
                    {
                        flag = false;
                        this.index++;
                        this.SkipDigits(false);
                    }
                    if ((this.index < this.length) && ((this.buffer[this.index] == 'E') || (this.buffer[this.index] == 'e')))
                    {
                        flag = false;
                        this.index++;
                        this.SkipDigits(true);
                    }
                }
                if (flag && (this.index <= (startIndex + 8)))
                {
                    int num3 = 1;
                    if (this.buffer[startIndex] == '+')
                    {
                        startIndex++;
                    }
                    else if (this.buffer[startIndex] == '-')
                    {
                        startIndex++;
                        num3 = -1;
                    }
                    int num2 = 0;
                    while (startIndex < this.index)
                    {
                        num2 = (num2 * 10) + (this.buffer[startIndex] - '0');
                        startIndex++;
                    }
                    return (Float)(num2 * num3);
                }
                string str = this.buffer.Substring(startIndex, this.index - startIndex);
                try
                {
                    num4 = Convert.ToDouble(str, CultureInfo.InvariantCulture);
                }
                catch (FormatException)
                {
                    throw new FormatException();
                }
                return (Float)num4;
            }

            private void SkipDigits(bool signAllowed)
            {
                if ((signAllowed) && (this.index < this.length) && ((this.buffer[this.index] == '-') || (this.buffer[this.index] == '+')))
                {
                    this.index++;
                }
                while ((this.index < this.length) && (this.buffer[this.index] >= '0') && (this.buffer[this.index] <= '9'))
                {
                    this.index++;
                }
            }

            private bool IsNumber(bool allowComma)
            {
                bool flag = this.SkipWhitespace(allowComma);
                if (this.index < this.length)
                {
                    this.token = this.buffer[this.index];
                    if ((this.token == '.') || (this.token == '-') || (this.token == '+') || (this.token >= '0') && (this.token <= '9') || (this.token == 'I') || (this.token == 'N'))
                    {
                        return true;
                    }
                }
                if (flag)
                {
                    throw new FormatException();
                }
                return false;
            }

            private bool SkipWhitespace(bool allowComma)
            {
                bool comma = false;
                while (this.index < this.length)
                {
                    char c = this.buffer[this.index];
                    switch (c)
                    {
                        case '\t':
                        case '\n':
                        case '\r':
                        case ' ':
                            break;

                        case ',':
                            if (allowComma)
                            {
                                comma = true;
                                allowComma = false;
                            }
                            else
                            {
                                throw new FormatException();
                            }
                            break;

                        default:
                            if ((c > ' ') && (c <= 'z'))
                            {
                                return comma;
                            }
                            if (!char.IsWhiteSpace(c))
                            {
                                return comma;
                            }
                            break;
                    }

                    this.index++;
                }

                return false;
            }

            private void BeginFigure(PointType startPoint)
            {
                this.FinishFigure(false);
                this.EnsureFigure();

                this.figure.StartPoint = startPoint;
                this.figure.IsFilled = true;
            }

            private void EnsureFigure()
            {
                if (this.figure == null)
                {
                    this.figure = new Xaml.Media.PathFigure();
                }
            }

            private void FinishFigure(bool figureExplicitlyClosed)
            {
                if (this.figure != null)
                {
                    if (figureExplicitlyClosed)
                    {
                        this.figure.IsClosed = true;
                    }

                    this.geometry.Figures.Add(this.figure);
                    this.figure = null;
                }
            }

            private void LineTo(PointType point)
            {
                Xaml.Media.LineSegment lineSegment = new Xaml.Media.LineSegment();
                lineSegment.Point = point;
                this.figure.Segments.Add(lineSegment);
            }

            private void BezierTo(PointType point1, PointType point2, PointType point3)
            {
                Xaml.Media.BezierSegment bezierSegment = new Xaml.Media.BezierSegment();
                bezierSegment.Point1 = point1;
                bezierSegment.Point2 = point2;
                bezierSegment.Point3 = point3;
                this.figure.Segments.Add(bezierSegment);
            }

            private void QuadraticBezierTo(PointType point1, PointType point2)
            {
                Xaml.Media.QuadraticBezierSegment bezierSegment = new Xaml.Media.QuadraticBezierSegment();
                bezierSegment.Point1 = point1;
                bezierSegment.Point2 = point2;
                this.figure.Segments.Add(bezierSegment);
            }

            private void ArcTo(SizeType size, double rotationAngle, bool isLargeArc, Xaml.Media.SweepDirection sweepDirection, PointType point)
            {
                Xaml.Media.ArcSegment arcSegment = new Xaml.Media.ArcSegment();
                arcSegment.Size = size;
                arcSegment.RotationAngle = rotationAngle;
                arcSegment.IsLargeArc = isLargeArc;
                arcSegment.SweepDirection = sweepDirection;
                arcSegment.Point = point;
                this.figure.Segments.Add(arcSegment);
            }

            private PointType GetSmoothBeizerFirstPoint()
            {
                // If there is no last segment or it is not a beizer segment then the point we 
                // are looking for is the current point (lastPoint). Otherwide the point is the
                // second control point of the last beizer segment reflected relatively current point
                PointType point = this.lastPoint;
                if (this.figure.Segments.Count() > 0)
                {
                    Xaml.Media.BezierSegment bezierSegment = this.figure.Segments.GetItem(this.figure.Segments.Count() - 1) as Xaml.Media.BezierSegment;
                    if (bezierSegment != null)
                    {
                        PointType pointToReflect = bezierSegment.Point2;
                        point.X += this.lastPoint.X - pointToReflect.X;
                        point.Y += this.lastPoint.Y - pointToReflect.Y;
                    }
                }
                return point;
            }
        }
    }

    // Not unit tested
    public class RoutedEventConverter : ConvertUsingXamlReaderConverter<Xaml.RoutedEvent>
    {
        private static Type element;
        private static PropertyInfo loaded;

        protected override PropertyInfo KnownProperty
        {
            get
            {
                if (loaded == null)
                {
                    loaded = PlatformMetadata.GetPropertyInfo(element, "Loaded");
                }
                return loaded;
            }
        }

        protected override Type KnownType
        {
            get
            {
                if (element == null)
                {
                    element = typeof(Xaml.Shapes.Ellipse);
                }
                return element;
            }
        }
    }

    public class PropertyPathConverter : ConvertToFromStringConverter
    {
        public override object ConvertFromStringInternal(string value, IFormatProvider provider)
        {
            return new Xaml.PropertyPath(value);
        }

        public override string ConvertToStringInternal(object value, IFormatProvider formatProvider)
        {
            return (value as Xaml.PropertyPath)?.Path;
        }
    }

#endregion
}
#endif