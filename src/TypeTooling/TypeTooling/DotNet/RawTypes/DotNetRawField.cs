namespace TypeTooling.DotNet.RawTypes {
    public abstract class DotNetRawField {
        public abstract string Name { get; }

        public abstract DotNetRawType FieldType { get;  }

        public abstract bool IsStatic { get; }

        public abstract bool IsPublic { get; }
    }
}
