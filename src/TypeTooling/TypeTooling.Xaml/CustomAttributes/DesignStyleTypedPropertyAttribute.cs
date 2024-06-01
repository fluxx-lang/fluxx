// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace TypeTooling.Xaml.CustomAttributes
{
    /// <summary>
    /// Replacement for StyleTypedPropertyAttribute to identify tarted type via string rather than Type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    [DebuggerDisplay("{Property}: {StyleTargetType}")]
    public sealed class DesignStyleTypedPropertyAttribute : Attribute
    {
        public DesignStyleTypedPropertyAttribute()
        {
            this.Property = string.Empty;
            this.StyleTargetType = string.Empty;
        }

        public string Property { get; set; }

        public string StyleTargetType { get; set; }
    }
}
