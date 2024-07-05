using System.Reflection;

namespace TypeTooling.DotNet.RawTypes.Reflection
{
    public class ReflectionDotNetRawCustomAttribute : DotNetRawCustomAttribute
    {
        private readonly CustomAttributeData _customAttributeData;

        public ReflectionDotNetRawCustomAttribute(CustomAttributeData customAttributeData)
        {
            this._customAttributeData = customAttributeData;
        }

        public override DotNetRawType AttributeType => new ReflectionDotNetRawType(this._customAttributeData.AttributeType);

        public override object? GetNamedArgumentValue(string argumentName)
        {
            foreach (CustomAttributeNamedArgument namedArgument in this._customAttributeData.NamedArguments)
                if (namedArgument.MemberName == argumentName)
                    return namedArgument.TypedValue.Value;
            }

            return null;
        }

        public override int GetPositionalArgumentCount()
        {
            return this._customAttributeData.ConstructorArguments.Count;
        }

        public override object GetPositionalArgumentValue(int index)
        {
            object value = this._customAttributeData.ConstructorArguments[index].Value;
            return value;
        }

        public CustomAttributeData CustomAttributeData => this._customAttributeData;
    }
}
