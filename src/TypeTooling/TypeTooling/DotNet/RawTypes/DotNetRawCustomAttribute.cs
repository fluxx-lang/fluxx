namespace TypeTooling.DotNet.RawTypes {
    public abstract class DotNetRawCustomAttribute {
        public abstract DotNetRawType AttributeType { get;  }

        public abstract object? GetNamedArgumentValue(string argumentName);

        public abstract int GetPositionalArgumentCount();

        public abstract object GetPositionalArgumentValue(int index);
    }
}
