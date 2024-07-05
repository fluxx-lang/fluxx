namespace TypeTooling
{
    public class PropertyValue<TProperty, TValue>
    {
        public TProperty Property { get; }
        public TValue Value { get; }

        public PropertyValue(TProperty property, TValue value)
        {
            Property = property;
            Value = value;
        }

        public PropertyValue<TOtherProperty, TValue> WithProperty<TOtherProperty>(TOtherProperty otherProperty) =>
            new PropertyValue<TOtherProperty, TValue>(otherProperty, Value);
    }
}
