// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System;

namespace TypeTooling.Xaml.CustomAttributes
{
    /// <summary>
    /// Attribute to indicate that a non-content property should be displayed in the the object tree.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class AlternateContentPropertyAttribute : Attribute
    {
    }
}
