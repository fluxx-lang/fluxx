using System.Reflection;

namespace Fluxx.DotNet
{
    public abstract class DotNetEnumerableType
    {
        public abstract TypeInfo GetElementType();

        public abstract void Add(object list, object element);
    }
}
