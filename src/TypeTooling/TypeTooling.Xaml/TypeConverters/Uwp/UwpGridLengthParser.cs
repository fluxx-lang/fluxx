using System;
using System.Globalization;
using TypeTooling.CodeGeneration;
using TypeTooling.CodeGeneration.Expressions;
using TypeTooling.DotNet.CodeGeneration;
using TypeTooling.DotNet.RawTypes;
using TypeTooling.Types;

// This code came from VS/src/Xaml/Designer/Source/UwpDesigner/Utility/XamlTypeConverters.cs

namespace TypeTooling.Xaml.TypeConverters.Uwp {
    public class UwpGridLengthParser : CustomLiteralParser {
        private readonly string? _initializationError = null;
        private readonly DotNetRawType? _doubleType;
        private readonly DotNetRawType? _gridLengthHelperType;
        private readonly DotNetRawType? _gridUnitTypeType;

        public UwpGridLengthParser(XamlTypeToolingProvider typeToolingProvider) {
            try {
                _doubleType = typeToolingProvider.GetRequiredRawType("System.Double");
                _gridLengthHelperType = typeToolingProvider.GetRequiredRawType("Windows.UI.Xaml.GridLengthHelper");
                _gridUnitTypeType = typeToolingProvider.GetRequiredRawType("Windows.UI.Xaml.GridUnitType");
            }
            catch (UserViewableException e) {
                _initializationError = "GridLength custom literal initialization error: " + e.Message;
            }
        }

        public override CustomLiteral Parse(string value) {
            if (_initializationError != null)
                return CustomLiteral.SingleError(_initializationError);

            value = value.Trim();
            if (string.Compare(value, "auto", true, CultureInfo.InvariantCulture) == 0)
                return new CustomLiteral(DotNetCode.Property(_gridLengthHelperType!, null, "Auto"));   // Call: GridLengthHelper.Auto

            ExpressionCode expression;
            if (value[value.Length - 1] == '*') {
                double numericValue;
                if (value == "*")
                    numericValue = 1;
                else {
                    string numberPortion = value.Substring(0, value.Length - 1);
                    try {
                        numericValue = Double.Parse(numberPortion);
                    }
                    catch (Exception) {
                        return CustomLiteral.SingleError($"'{numberPortion}' isn't a valid floating point number");
                    }
                }

                // Call: GridLengthHelper.FromValueAndType(number, GridUnitType.Star)
                expression = DotNetCode.CallStatic(_gridLengthHelperType!, "FromValueAndType",
                    new[] {_doubleType!, _gridUnitTypeType!},
                    Code.DoubleLiteral(numericValue), DotNetCode.EnumValue(_gridUnitTypeType!, "Star"));  
            }
            else {
                double numericValue;
                try {
                    numericValue = Double.Parse(value);
                }
                catch (Exception) {
                    return CustomLiteral.SingleError("Must be a floating point number, '*', number followed by '*', or 'Auto'");
                }

                // Call: GridLengthHelper.FromPixels(number)
                expression = DotNetCode.CallStatic(_gridLengthHelperType!, "FromPixels",
                    new[] { _doubleType! },
                    Code.DoubleLiteral(numericValue));
            }

            return new CustomLiteral(expression);
        }
    }
}
