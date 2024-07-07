namespace TypeTooling
{
    public class PropertyValue<TProperty, TValue>(TProperty property, TValue value)
    {
        public TProperty Property { get; } = property;
        public TValue Value { get; } = value;

        public PropertyValue<TOtherProperty, TValue> WithProperty<TOtherProperty>(TOtherProperty otherProperty) =>
            new PropertyValue<TOtherProperty, TValue>(otherProperty, this.Value);
    }
}
