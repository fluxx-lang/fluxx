// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System;

namespace TypeTooling.Xaml.CustomAttributes
{
    /// <summary>
    /// Indicates which property of a type is the XAML content property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ContentPropertyAttribute : Attribute
    {
        private readonly string _name;

        public ContentPropertyAttribute(string name)
        {
            _name = name;
        }

        public string Name => _name;
    }
}
