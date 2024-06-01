namespace TypeTooling.Xaml.TypeConverters.UwpWpf
{
    // Wpf and SL use FontStyles helper class while Win8+ platforms use FontStyle enum.

    internal class UwpWpfFontStyleConverter : TypeResolvingEnumTypeConverter
    {
        public UwpWpfFontStyleConverter(XamlTypeToolingProvider xamlTypeToolingProvider) : base(xamlTypeToolingProvider)
        {
        }

        protected override string GetTypeSymbol()
        {
            if (Platform == Platform.Uwp)
                return "Windows.UI.Text.FontStyle";
            else return "System.Windows.FontStyles";
        }
    }
}
