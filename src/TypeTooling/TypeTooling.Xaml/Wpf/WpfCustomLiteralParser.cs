using System;
using TypeTooling.CodeGeneration;
using TypeTooling.DotNet.CodeGeneration;
using TypeTooling.DotNet.RawTypes;
using TypeTooling.Types;

namespace TypeTooling.Xaml.Wpf {
    public class WpfCustomLiteralParser : CustomLiteralParser {
        private readonly string? _initializationError;
        private readonly DotNetRawType? _typeConverterRawType;
        private readonly DotNetRawMethod? _convertFromMethod;

        public WpfCustomLiteralParser(XamlTypeToolingProvider xamlTypeToolingProvider, Type typeConverterType) {
            _initializationError = null;

            try {
                _typeConverterRawType = xamlTypeToolingProvider.GetRequiredRawType(typeConverterType.FullName);

                DotNetRawType objectRawType = xamlTypeToolingProvider.GetRequiredRawType("System.Object");

                _convertFromMethod =
                    _typeConverterRawType.GetRequiredInstanceMethod("ConvertFrom", new[] {objectRawType});
            }
            catch (UserViewableException e) {
                _initializationError = "Forms custom literal initialization error: " + e.Message;
            }
        }

        public override CustomLiteral Parse(string literal) {
            if (_initializationError != null)
                return CustomLiteral.SingleError(_initializationError);

            try {
                MethodCallCode convertMethodCall = Code.Call(DotNetCode.New(_typeConverterRawType!),
                    _convertFromMethod!, Code.StringLiteral(literal));

                return new CustomLiteral(convertMethodCall);
            }
            catch (Exception e) {
                return CustomLiteral.SingleError(e.Message);
            }
        }
    }
}
