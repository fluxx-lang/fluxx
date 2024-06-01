using System.Collections.Generic;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace TypeTooling.DotNet.RawTypes.Roslyn {
    public class RoslynDotNetRawCustomAttribute : DotNetRawCustomAttribute {
        private readonly AttributeData _attributeData;

        public RoslynDotNetRawCustomAttribute(AttributeData attributeData) {
            _attributeData = attributeData;
        }

        public override DotNetRawType AttributeType => new RoslynDotNetRawType(_attributeData.AttributeClass);

        public override object? GetNamedArgumentValue(string argumentName) {
            foreach (KeyValuePair<string, TypedConstant> namedArgument in _attributeData.NamedArguments) {
                if (namedArgument.Key == argumentName) {
                    TypedConstant value = namedArgument.Value;
                    return value.Value;
                }
            }

            return null;
        }

        public override int GetPositionalArgumentCount() {
            return _attributeData.ConstructorArguments.Length;
        }

        public override object GetPositionalArgumentValue(int index) {
            TypedConstant value = _attributeData.ConstructorArguments[index];
            return value.Value;
        }

        public AttributeData AttributeData => _attributeData;
    }
}
