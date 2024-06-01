namespace TypeTooling.Types.PredefinedTypes {
    public class BooleanType : PredefinedType {
        public static BooleanType Instance = new BooleanType();

        public override string FullName => nameof(BooleanType);
    }
}
