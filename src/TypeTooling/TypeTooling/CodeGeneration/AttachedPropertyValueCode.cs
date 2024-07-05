using TypeTooling.CodeGeneration.Expressions;
using TypeTooling.Types;

namespace TypeTooling.CodeGeneration
{
    public class AttachedPropertyValueCode
    {
        public AttachedProperty AttachedProperty { get; }
        public ExpressionCode Value { get; }

        public AttachedPropertyValueCode(AttachedProperty attachedProperty, ExpressionCode value)
        {
            this.AttachedProperty = attachedProperty;
            this.Value = value;
        }
    }
}
