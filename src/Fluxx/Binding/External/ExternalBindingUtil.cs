using TypeTooling;
using TypeTooling.DotNet.RawTypes;
using TypeTooling.Types;
using TypeTooling.Types.PredefinedTypes;

namespace Fluxx.Binding.External
{
    public class ExternalBindingUtil
    {
        public static TypeBinding DotNetTypeToTypeBinding(FamlProject project, DotNetRawType type)
        {
            string typeFullName = type.FullName;

            if (typeFullName.Equals("System.Int32"))
            {
                return BuiltInTypeBinding.Int;
            }
            else if (typeFullName.Equals("System.Double"))
            {
                return BuiltInTypeBinding.Double;
            }
            else if (typeFullName.Equals("System.Boolean"))
            {
                return BuiltInTypeBinding.Bool;
            }
            else if (typeFullName.Equals("System.String"))
            {
                return BuiltInTypeBinding.String;
            }
            else if (typeFullName.StartsWith("IList")) // TODO: FIX THIS UP, TO RESPECT COMPONENT TYPE
            {
                return new SequenceTypeBinding(BuiltInTypeBinding.String);
            }
            else
            {
                return new ExternalObjectTypeBinding(project, type);
            }
        }

        public static TypeBinding TypeToolingTypeToTypeBinding(FamlProject project, TypeToolingType typeToolingType)
        {
            if (typeToolingType is BooleanType)
            {
                return BuiltInTypeBinding.Bool;
            }
            else if (typeToolingType is IntegerType)
            {
                return BuiltInTypeBinding.Int;
            }
            else if (typeToolingType is StringType)
            {
                return BuiltInTypeBinding.String;
            }
            else if (typeToolingType is UITextType)
            {
                return BuiltInTypeBinding.String;
            }
            else if (typeToolingType is SequenceType)
            {
                return new SequenceTypeBinding(BuiltInTypeBinding.String);
            }
            else if (typeToolingType is ObjectType objectType)
            {
                return new ExternalObjectTypeBinding(project, objectType);
            }
            else if (typeToolingType is EnumType enumType)
            {
                return new ExternalEnumTypeBinding(project, enumType);
            }
            else
            {
                throw new UserViewableException($"Unknown kind of TypeTooling type: {typeToolingType.FullName}");
            }
        }
    }
}
