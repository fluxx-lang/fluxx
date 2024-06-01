// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System;
using TypeTooling.Types;
using TypeTooling.Xaml.TypeConverters;

namespace TypeTooling.Xaml.CustomAttributes
{
    /// <summary>
    /// Specifies what type to use as a converter for the object this attribute is bound to.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public sealed class CustomLiteralParserAttribute : Attribute {
        private readonly CustomLiteralParser _customLiteralParser;

        public CustomLiteralParserAttribute(CustomLiteralParser customLiteralParser) {
            this._customLiteralParser = customLiteralParser;
        }

        public CustomLiteralParser CustomLiteralParser => _customLiteralParser;
    }
}
