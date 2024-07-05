using TypeTooling.RawTypes;

namespace TypeTooling.DotNet.RawTypes
{
    public abstract class DotNetRawConstructor : RawConstructor
    {
        public abstract DotNetRawParameter[] GetParameters();
    }
}
