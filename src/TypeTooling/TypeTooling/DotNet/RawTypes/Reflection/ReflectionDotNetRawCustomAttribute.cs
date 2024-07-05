using System.Reflection;

namespace TypeTooling.DotNet.RawTypes.Reflection
{
    public class ReflectionDotNetRawCustomAttribute : DotNetRawCustomAttribute
    {
        private readonly CustomAttributeData _customAttributeData;

        public ReflectionDotNetRawCustomAttribute(CustomAttributeData customAttributeData)
        {
            _customAttributeData = customAttributeData;
        }

        public override DotNetRawType AttributeType => new ReflectionDotNetRawType(_customAttributeData.AttributeType);

        public override object? GetNamedArgumentValue(string argumentName)
        {
            foreach (CustomAttributeNamedArgument namedArgument in _customAttributeData.NamedArguments)
                if (namedArgument.MemberName == argumentName)
                    return namedArgument.TypedValue.Value;

            return null;
        }

        public override int GetPositionalArgumentCount()
        {
            return _customAttributeData.ConstructorArguments.Count;
        }

        public override object GetPositionalArgumentValue(int index)
        {
            object value = _customAttributeData.ConstructorArguments[index].Value;
            return value;
        }

        public CustomAttributeData CustomAttributeData => _customAttributeData;
    }
}
