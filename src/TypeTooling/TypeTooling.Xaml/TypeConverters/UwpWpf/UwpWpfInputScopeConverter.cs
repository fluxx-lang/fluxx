// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

namespace TypeTooling.Xaml.TypeConverters.UwpWpf
{
    internal class UwpWpfInputScopeConverter : TypeResolvingEnumTypeConverter
    {
        public UwpWpfInputScopeConverter(XamlTypeToolingProvider xamlTypeToolingProvider) : base(xamlTypeToolingProvider) {
        }

        protected override string GetTypeSymbol()
        {
            return "Windows.UI.Xaml.Input.InputScopeNameValue";
        }
    }
}
