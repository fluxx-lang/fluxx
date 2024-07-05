namespace TypeTooling.Types
{
    public abstract class InterpretedObjectCreator
    {
        public abstract object Create(object[] values, int startOffset);
    }
}
