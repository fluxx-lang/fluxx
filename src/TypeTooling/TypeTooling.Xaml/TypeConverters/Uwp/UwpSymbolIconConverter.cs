// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using TypeTooling.Xaml.TypeConverters.UwpWpf;

namespace TypeTooling.Xaml.TypeConverters.Uwp
{
    internal class UwpSymbolIconConverter : TypeResolvingEnumTypeConverter
    {
        public UwpSymbolIconConverter(XamlTypeToolingProvider xamlTypeToolingProvider) : base(xamlTypeToolingProvider) {
        }

        protected override string GetTypeSymbol()
        {
            return "Windows.UI.Xaml.Controls.Symbol";
        }
    }
}
