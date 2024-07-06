using TypeTooling.CodeGeneration.Expressions;
using TypeTooling.Types;

namespace TypeTooling.CodeGeneration
{
    public class AttachedPropertyValueCode(AttachedProperty attachedProperty, ExpressionCode value)
    {
        public AttachedProperty AttachedProperty { get; } = attachedProperty;
        public ExpressionCode Value { get; } = value;
    }
}
