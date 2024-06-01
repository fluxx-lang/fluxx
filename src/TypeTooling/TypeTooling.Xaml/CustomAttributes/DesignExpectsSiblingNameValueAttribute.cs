// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System;

namespace TypeTooling.Xaml.CustomAttributes
{
    /// <summary>
    /// Annotates properties that represent a reference to the name of a sibling element in the scope of the parent.
    /// For example RelativePanel.AlignBottomWith="x", where x is the name of a child in the panel.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DesignExpectsSiblingNameValueAttribute : Attribute
    {
    }
}
