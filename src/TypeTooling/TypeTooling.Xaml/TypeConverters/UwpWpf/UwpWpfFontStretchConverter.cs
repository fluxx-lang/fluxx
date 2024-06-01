// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

namespace TypeTooling.Xaml.TypeConverters.UwpWpf
{
    // Wpf and SL use FontStretches helper class while Win8+ platforms use FontStretch enum.

    internal class UwpWpfFontStretchConverter : TypeResolvingEnumTypeConverter
    {
        public UwpWpfFontStretchConverter(XamlTypeToolingProvider xamlTypeToolingProvider) : base(xamlTypeToolingProvider)
        {
        }

        protected override string GetTypeSymbol()
        {
            if (Platform == Platform.Uwp)
                return "Windows.UI.Text.FontStretch";
            else return "System.Windows.FontStretches";
        }
    }
}
