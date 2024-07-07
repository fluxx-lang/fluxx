using Fluxx.Api;
using Fluxx.Syntax;
using Fluxx.Syntax.Type;

namespace Fluxx.Binding.Resolver
{
    public abstract class BindingResolver
    {
        public abstract TypeBindingResult FindTypeBindingForType(QualifiableName typeName);

        public abstract FunctionBinding ResolveFunctionBinding(TypeBinding? thisArgumentTypeBinding,
            QualifiableName functionName, SyntaxNode nameSyntaxForErrors);

        public abstract TypeBinding ResolveObjectTypeBinding(ObjectTypeReferenceSyntax objectTypeReferenceSyntax);

        public abstract AttachedTypeBinding? ResolveAttachedTypeBinding(QualifiableName typeName);

        public PropertyBinding ResolvePropertyBinding(TypeBinding expressionTypeBinding, NameSyntax propertyNameSyntax)
        {
            Name propertyName = propertyNameSyntax.Name;

            if (!(expressionTypeBinding is ObjectTypeBinding objectTypeBinding))
            {
                propertyNameSyntax.AddError(
                    $"Type {expressionTypeBinding.TypeName} isn't an object type, so reference to {propertyName} property is invalid");
                return new InvalidPropertyBinding(propertyName);
            }

            PropertyBinding propertyBinding = objectTypeBinding.GetPropertyBinding(propertyName);
            if (propertyBinding == null)
            {
                propertyNameSyntax.AddError(
                    $"Type {expressionTypeBinding.TypeName} doesn't have property {propertyName}");
                return new InvalidPropertyBinding(propertyName);
            }

            return propertyBinding;
        }
    }
}
