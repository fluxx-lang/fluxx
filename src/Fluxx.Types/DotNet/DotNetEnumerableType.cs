using System.Reflection;

namespace Faml.DotNet {
    public abstract class DotNetEnumerableType {
        public abstract TypeInfo GetElementType();

        public abstract void Add(object list, object element);
    }
}