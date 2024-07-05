namespace TypeTooling.Types
{
    public abstract class SequenceType : TypeToolingType
    {
        public abstract TypeToolingType ElementType { get; }
    }
}
