using TypeTooling.RawTypes;

namespace TypeTooling.DotNet.RawTypes
{
    public abstract class DotNetRawMethod : RawMethod
    {
        public abstract DotNetRawParameter[] GetParameters();

        public abstract DotNetRawType ReturnType { get;  }

        public abstract string Name { get; }
            
        public abstract bool IsStatic { get; }
    }
}
