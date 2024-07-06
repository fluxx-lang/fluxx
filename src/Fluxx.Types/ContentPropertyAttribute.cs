using System;

namespace Faml
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ContentPropertyAttribute(string name) : Attribute
    {
        /// <summary>Gets the name of the content property</summary>
        /// <value>A string representing the name of the content property.</value>
        public string Name { get; private set; } = name;
    }
}
