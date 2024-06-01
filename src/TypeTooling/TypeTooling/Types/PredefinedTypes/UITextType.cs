namespace TypeTooling.Types.PredefinedTypes {
    public class UITextType : PredefinedType {
        public static UITextType Instance = new UITextType();

        public override string FullName => nameof(UITextType);
    }
}
