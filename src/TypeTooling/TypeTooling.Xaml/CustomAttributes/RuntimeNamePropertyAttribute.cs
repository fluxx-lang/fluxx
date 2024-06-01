// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System;

namespace TypeTooling.Xaml.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class RuntimeNamePropertyAttribute : Attribute
    {
        private readonly string _name;

        public RuntimeNamePropertyAttribute(string name)
        {
            _name = name;
        }

        public string Name => _name;
    }
}
