namespace TypeTooling.Types.PredefinedTypes
{
    public class IntegerType : PredefinedType
    {
        public static IntegerType Instance = new IntegerType();

        public override string FullName => nameof(IntegerType);
    }
}
