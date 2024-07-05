using TypeTooling.DotNet.RawTypes;
using TypeTooling.DotNet.RawTypes.Reflection;
using TypeTooling.Types;

namespace TypeTooling.DotNet.Types
{
    public class DotNetObjectPropertyReader : ObjectPropertyReader
    {
        private readonly DotNetRawProperty _rawProperty;

        public DotNetObjectPropertyReader(DotNetRawProperty rawProperty)
        {
            this._rawProperty = rawProperty;
        }

        public override object Get(object obj)
        {
            return ((ReflectionDotNetRawProperty)this._rawProperty).PropertyInfo.GetValue(obj);
        }
    }
}
