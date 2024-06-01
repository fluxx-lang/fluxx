using System;
using TypeTooling;
using TypeTooling.CompanionType;
using TypeTooling.Types;

namespace Faml.Lang
{
    public sealed class Color {
        public byte Red { get; }
        public byte Green { get; }
        public byte Blue { get; }
        public byte Alpha { get; }

        public Color(byte red, byte green, byte blue, byte alpha) {
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
            this.Alpha = alpha;
        }
    }

    public class ColorTypeTooling : ICustomLiteralParser {
        CustomLiteral ICustomLiteralParser.Parse(string value) {
            throw new NotImplementedException("ColorTypeTooling Parse needs to be fully converted to return Code object");
#if false

            if (value == "transparent")
                return null;

            if (!value.StartsWith("#") || value.Length != 7)
                return CustomLiteral.SingleError("Color must be of the form #xxxxxx");

            string hex = value.Substring(1);
            foreach (char c in hex) {
                if (! ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F')))
                    return CustomLiteral.SingleError("Color must be of the form #xxxxxx, where x is a valid hex digit (0-9, a-f, or A-F)");
            }

            return null;
#endif
        }

#if false
        object ICustomLiteralParser.Create(string value) {
            if (value == "transparent")
                return new Color(0, 0, 0, 0);
            else {
                string hex = value.Substring(1);

                return new Color(
                    byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                    byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                    byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber), 0xFF);
            }
        }
#endif
    }
}
