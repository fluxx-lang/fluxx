namespace TypeTooling.Types.PredefinedTypes
{
    public class DoubleType : PredefinedType
    {
        public static DoubleType Instance = new DoubleType();

        public override string FullName => nameof(DoubleType);
    }
}
