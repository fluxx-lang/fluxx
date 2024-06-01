// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

namespace TypeTooling.Xaml.TypeConverters.UwpWpf
{
    internal class UwpWpfFontWeightConverter : TypeResolvingEnumTypeConverter
    {
        public UwpWpfFontWeightConverter(XamlTypeToolingProvider xamlTypeToolingProvider) : base(xamlTypeToolingProvider) {
        }

        protected override string GetTypeSymbol()
        {
            return "Windows.UI.Text.FontWeights";
        }
    }

}
