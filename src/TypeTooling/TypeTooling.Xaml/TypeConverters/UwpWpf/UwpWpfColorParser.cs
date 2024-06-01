//------------------------------------------------------------------------------
//  Microsoft Avalon
//  Copyright (c) Microsoft Corporation, 2001
//
//  File:       Parsers.cs
//  Synopsis: Implements class Parsers for internal use of type converters
//  Created: 06/13/2002
//
//------------------------------------------------------------------------------

using System;
using System.ComponentModel;

// This code was copied from .NET Framework 4.6.2 source files:
// dotnet462RTM\Source\wpf\src\Core\CSharp\System\Windows\Media\Parsers.cs
// dotnet462RTM\Source\wpf\src\Core\CSharp\System\Windows\Media\KnownColors.cs

namespace TypeTooling.Xaml.TypeConverters.UwpWpf {
    internal static class UwpWpfColorParser {
        private const int s_zeroChar = (int)'0';
        private const int s_aLower = (int)'a';
        private const int s_aUpper = (int)'A';

        private static int ParseHexChar(char c) {
            int intChar = (int)c;

            if ((intChar >= s_zeroChar) && (intChar <= (s_zeroChar + 9))) {
                return (intChar - s_zeroChar);
            }

            if ((intChar >= s_aLower) && (intChar <= (s_aLower + 5))) {
                return (intChar - s_aLower + 10);
            }

            if ((intChar >= s_aUpper) && (intChar <= (s_aUpper + 5))) {
                return (intChar - s_aUpper + 10);
            }
            throw new FormatException("Illegal token");
        }

        private static UwpWpfColor ParseHexColor(string trimmedColor) {
            int a, r, g, b;
            a = 255;

            if (trimmedColor.Length > 7) {
                a = ParseHexChar(trimmedColor[1]) * 16 + ParseHexChar(trimmedColor[2]);
                r = ParseHexChar(trimmedColor[3]) * 16 + ParseHexChar(trimmedColor[4]);
                g = ParseHexChar(trimmedColor[5]) * 16 + ParseHexChar(trimmedColor[6]);
                b = ParseHexChar(trimmedColor[7]) * 16 + ParseHexChar(trimmedColor[8]);
            }
            else if (trimmedColor.Length > 5) {
                r = ParseHexChar(trimmedColor[1]) * 16 + ParseHexChar(trimmedColor[2]);
                g = ParseHexChar(trimmedColor[3]) * 16 + ParseHexChar(trimmedColor[4]);
                b = ParseHexChar(trimmedColor[5]) * 16 + ParseHexChar(trimmedColor[6]);
            }
            else if (trimmedColor.Length > 4) {
                a = ParseHexChar(trimmedColor[1]);
                a = a + a * 16;
                r = ParseHexChar(trimmedColor[2]);
                r = r + r * 16;
                g = ParseHexChar(trimmedColor[3]);
                g = g + g * 16;
                b = ParseHexChar(trimmedColor[4]);
                b = b + b * 16;
            }
            else {
                r = ParseHexChar(trimmedColor[1]);
                r = r + r * 16;
                g = ParseHexChar(trimmedColor[2]);
                g = g + g * 16;
                b = ParseHexChar(trimmedColor[3]);
                b = b + b * 16;
            }

            return (UwpWpfColor.FromArgb((byte)a, (byte)r, (byte)g, (byte)b));
        }

        internal const string s_ContextColor = "ContextColor ";
        internal const string s_ContextColorNoSpace = "ContextColor";

        private static UwpWpfColor ParseContextColor(string trimmedColor, IFormatProvider formatProvider, ITypeDescriptorContext? context) {
            if (!trimmedColor.StartsWith(s_ContextColor, StringComparison.OrdinalIgnoreCase)) {
                throw new FormatException("Illegal token");
            }

            throw new FormatException("Context colors aren't currently supported");
#if false
            string tokens = trimmedColor.Substring(s_ContextColor.Length);
            tokens = tokens.Trim();
            string[] preSplit = tokens.Split(new Char[] { ' ' });
            if (preSplit.GetLength(0) < 2) {
                throw new FormatException("Illegal token");
            }

            tokens = tokens.Substring(preSplit[0].Length);

            TokenizerHelper th = new TokenizerHelper(tokens, formatProvider);
            string[] split = tokens.Split(new Char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int numTokens = split.GetLength(0);

            float alpha = Convert.ToSingle(th.NextTokenRequired(), formatProvider);

            float[] values = new float[numTokens - 1];

            for (int i = 0; i < numTokens - 1; i++) {
                values[i] = Convert.ToSingle(th.NextTokenRequired(), formatProvider);
            }

            string profileString = preSplit[0];

            UriHolder uriHolder = TypeConverterHelper.GetUriFromUriContext(context, profileString);

            Uri profileUri;

            if (uriHolder.BaseUri != null) {
                profileUri = new Uri(uriHolder.BaseUri, uriHolder.OriginalUri);
            }
            else {
                profileUri = uriHolder.OriginalUri;
            }

            Color result = Color.FromAValues(alpha, values, profileUri);

            // If the number of color values found does not match the number of channels in the profile, we must throw
            if (result.ColorContext.NumChannels != values.Length) {
                throw new FormatException(SR.Get(SRID.Parsers_IllegalToken));
            }

            return result;
#endif
        }

        private static UwpWpfColor ParseScRgbColor(string trimmedColor, IFormatProvider formatProvider) {
            if (!trimmedColor.StartsWith("sc#", StringComparison.Ordinal)) {
                throw new FormatException("Illegal token");
            }

            throw new FormatException("ScRgb colors aren't currently supported");
#if false
            string tokens = trimmedColor.Substring(3, trimmedColor.Length - 3);

            // The tokenizer helper will tokenize a list based on the IFormatProvider.
            TokenizerHelper th = new TokenizerHelper(tokens, formatProvider);
            float[] values = new float[4];

            for (int i = 0; i < 3; i++) {
                values[i] = Convert.ToSingle(th.NextTokenRequired(), formatProvider);
            }

            if (th.NextToken()) {
                values[3] = Convert.ToSingle(th.GetCurrentToken(), formatProvider);

                // We should be out of tokens at this point
                if (th.NextToken()) {
                    throw new FormatException(SR.Get(SRID.Parsers_IllegalToken));
                }

                return Color.FromScRgb(values[0], values[1], values[2], values[3]);
            }
            else {
                return Color.FromScRgb(1.0f, values[0], values[1], values[2]);
            }
#endif
        }

        /// <summary>
        /// ParseColor
        /// <param name="color"> string with color description </param>
        /// <param name="formatProvider">IFormatProvider for processing string</param>
        /// </summary>
        internal static UwpWpfColor ParseColor(string color, IFormatProvider formatProvider) {
            return ParseColor(color, formatProvider, null);
        }

        /// <summary>
        /// ParseColor
        /// <param name="color"> string with color description </param>
        /// <param name="formatProvider">IFormatProvider for processing string</param>
        /// <param name="context">ITypeDescriptorContext</param>
        /// </summary>
        internal static UwpWpfColor ParseColor(string color, IFormatProvider formatProvider, ITypeDescriptorContext? context) {
            bool isPossibleKnowColor;
            bool isNumericColor;
            bool isScRgbColor;
            bool isContextColor;
            string trimmedColor = MatchColor(color, out isPossibleKnowColor, out isNumericColor, out isContextColor, out isScRgbColor);

            if ((isPossibleKnowColor == false) &&
                (isNumericColor == false) &&
                (isScRgbColor == false) &&
                (isContextColor == false)) {
                throw new FormatException("Illegal token");
            }

            //Is it a number?
            if (isNumericColor) {
                return ParseHexColor(trimmedColor);
            }
            else if (isContextColor) {
                return ParseContextColor(trimmedColor, formatProvider, context);
            }
            else if (isScRgbColor) {
                return ParseScRgbColor(trimmedColor, formatProvider);
            }
            else throw new FormatException("Illegal token");
        }

        internal static string MatchColor(string colorString, out bool isKnownColor, out bool isNumericColor, out bool isContextColor, out bool isScRgbColor) {

            string trimmedString = colorString.Trim();

            if (((trimmedString.Length == 4) ||
                 (trimmedString.Length == 5) ||
                 (trimmedString.Length == 7) ||
                 (trimmedString.Length == 9)) &&
                (trimmedString[0] == '#')) {
                isNumericColor = true;
                isScRgbColor = false;
                isKnownColor = false;
                isContextColor = false;
                return trimmedString;
            }
            else
                isNumericColor = false;

            if ((trimmedString.StartsWith("sc#", StringComparison.Ordinal) == true)) {
                isNumericColor = false;
                isScRgbColor = true;
                isKnownColor = false;
                isContextColor = false;
            }
            else {
                isScRgbColor = false;
            }

            if ((trimmedString.StartsWith(UwpWpfColorParser.s_ContextColor, StringComparison.OrdinalIgnoreCase) == true)) {
                isContextColor = true;
                isScRgbColor = false;
                isKnownColor = false;
                return trimmedString;
            }
            else {
                isContextColor = false;
                isKnownColor = true;
            }

            return trimmedString;
        }
    }
}
