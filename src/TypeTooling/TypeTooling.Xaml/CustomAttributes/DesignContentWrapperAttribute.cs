// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System;

namespace TypeTooling.Xaml.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class DesignContentWrapperAttribute : Attribute
    {
        public DesignContentWrapperAttribute(string contentWrapper)
        {
            this.ContentWrapper = contentWrapper;
        }

        public string ContentWrapper
        {
            get;
        }

        public override bool Equals(object obj)
        {
            DesignContentWrapperAttribute? other = obj as DesignContentWrapperAttribute;
            if (other == null)
            {
                return false;
            }

            return this.ContentWrapper == other.ContentWrapper;
        }

        public override int GetHashCode()
        {
            return this.ContentWrapper.GetHashCode();
        }
    }
}
