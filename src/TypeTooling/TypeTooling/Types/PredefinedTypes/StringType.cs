namespace TypeTooling.Types.PredefinedTypes {
    public class StringType : PredefinedType {
        public static StringType Instance = new StringType();

        public override string FullName => nameof(StringType);
    }
}
