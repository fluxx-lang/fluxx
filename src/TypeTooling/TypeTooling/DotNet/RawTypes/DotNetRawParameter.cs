namespace TypeTooling.DotNet.RawTypes {
    public abstract class DotNetRawParameter {
        public abstract string Name { get; }

        public abstract DotNetRawType ParameterType { get; }
    }
}
