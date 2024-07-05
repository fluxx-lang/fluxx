using System.Reflection;

namespace TypeTooling.DotNet.RawTypes.Reflection
{
    public class ReflectionDotNetRawCustomAttribute : DotNetRawCustomAttribute
    {
        private readonly CustomAttributeData customAttributeData;

        public ReflectionDotNetRawCustomAttribute(CustomAttributeData customAttributeData)
        {
            this.customAttributeData = customAttributeData;
        }

        public override DotNetRawType AttributeType => new ReflectionDotNetRawType(this.customAttributeData.AttributeType);

        public override object? GetNamedArgumentValue(string argumentName)
        {
            foreach (CustomAttributeNamedArgument namedArgument in this.customAttributeData.NamedArguments)
            {
                if (namedArgument.MemberName == argumentName)
                {
                    return namedArgument.TypedValue.Value;
                }
            }

            return null;
        }

        public override int GetPositionalArgumentCount()
        {
            return this.customAttributeData.ConstructorArguments.Count;
        }

        public override object GetPositionalArgumentValue(int index)
        {
            object value = this.customAttributeData.ConstructorArguments[index].Value;
            return value;
        }

        public CustomAttributeData CustomAttributeData => this.customAttributeData;
    }
}
