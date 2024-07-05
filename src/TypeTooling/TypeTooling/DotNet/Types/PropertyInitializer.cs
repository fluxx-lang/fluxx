namespace TypeTooling.DotNet.Types
{
    public abstract class PropertyInitializer
    {
        public abstract void Initialize(object obj, object propertyValue);
    }
}