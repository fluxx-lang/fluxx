using System;

namespace Faml
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ContentPropertyAttribute : Attribute {
        /// <summary>Gets the name of the content property</summary>
        /// <value>A string representing the name of the content property.</value>
        /// <remarks />
        public string Name { get; private set; }

        public ContentPropertyAttribute(string name) {
            this.Name = name;
        }
    }
}
