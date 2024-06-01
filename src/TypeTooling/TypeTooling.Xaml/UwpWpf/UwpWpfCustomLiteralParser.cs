using System;
using System.ComponentModel;
using TypeTooling.CodeGeneration;
using TypeTooling.DotNet.CodeGeneration;
using TypeTooling.DotNet.RawTypes;
using TypeTooling.Types;

namespace TypeTooling.Xaml.UwpWpf {
    public class UwpWpfCustomLiteralParser : CustomLiteralParser {
        private readonly TypeConverter _typeConverter;
        private readonly string? _initializationError;
        private readonly DotNetRawType? _typeConverterRawType;
        private readonly DotNetRawMethod? _convertFromInvariantStringMethod;


        public UwpWpfCustomLiteralParser(XamlTypeToolingProvider xamlTypeToolingProvider, TypeConverter typeConverter) {
            _initializationError = null;
            _typeConverter = typeConverter;

            try {
                _typeConverterRawType = xamlTypeToolingProvider.GetRequiredRawType(_typeConverter.GetType().FullName);

                DotNetRawType stringRawType = xamlTypeToolingProvider.GetRequiredRawType("System.String");

                _convertFromInvariantStringMethod =
                    _typeConverterRawType.GetRequiredInstanceMethod(nameof(TypeConverter.ConvertFromInvariantString),
                        new[] { stringRawType });
            }
            catch (UserViewableException e) {
                _initializationError = $"{typeConverter.GetType().Name} custom literal initialization error: " + e.Message;
            }
        }

        public override CustomLiteral Parse(string literal) {
            if (_initializationError != null)
                return CustomLiteral.SingleError(_initializationError);

            try {
                object value = _typeConverter.ConvertFromInvariantString(literal);
                if (value == null)
                    return CustomLiteral.SingleError($"Invalid literal value: {literal}");

                MethodCallCode convertMethodCall = Code.Call(
                    DotNetCode.New(_typeConverterRawType!),
                    _convertFromInvariantStringMethod!,
                    Code.StringLiteral(literal));

                return new CustomLiteral(convertMethodCall);
            }
            catch (Exception e) {
                return CustomLiteral.SingleError(e.Message);
            }
        }
    }
}
