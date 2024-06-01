// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System;

namespace TypeTooling.Xaml.CustomAttributes
{
    /// <summary>
    /// Specifies that an attached property has a browsable scope that extends to child elements in the logical tree.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class AttachedPropertyBrowsableForChildrenAttribute : Attribute
    {
    }
}
