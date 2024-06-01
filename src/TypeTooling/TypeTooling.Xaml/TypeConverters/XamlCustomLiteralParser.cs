using System.Collections.Generic;
using System.ComponentModel;
using TypeTooling.Types;

namespace TypeTooling.Xaml.TypeConverters
{
    public abstract class XamlCustomLiteralParser : CustomLiteralParser {
        private readonly XamlTypeToolingProvider _typeToolingProvider;


        protected XamlCustomLiteralParser(XamlTypeToolingProvider typeToolingProvider) {
            _typeToolingProvider = typeToolingProvider;
        }

        public XamlTypeToolingProvider XamlTypeToolingProvider => _typeToolingProvider;

        public abstract IReadOnlyCollection<string> GetCommonValues(ITypeDescriptorContext context);
    }
}