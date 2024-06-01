using System.Collections.Generic;
using System.Linq;
using TypeTooling.DotNet;
using TypeTooling.DotNet.RawTypes;
using TypeTooling.RawTypes;
using TypeTooling.Types;

namespace TypeTooling.Xaml {
    public abstract class XamlTypeToolingProvider : DotNetTypeToolingProvider {
        protected XamlTypeToolingProvider(TypeToolingEnvironment typeToolingEnvironment) : base(typeToolingEnvironment) {
        }

        public DotNetRawType? GetRawType(string typeName) {
            return (DotNetRawType?) TypeToolingEnvironment.GetRawType(typeName);
        }

        public DotNetRawType GetRequiredRawType(string typeName) {
            RawType rawType = TypeToolingEnvironment.GetRequiredRawType(typeName);
            if (! (rawType is DotNetRawType dotNetRawType))
                throw new UserViewableException($"Expected type {typeName} to be a .NET type, but a {rawType.GetType().FullName} type was found instead");

            return dotNetRawType;
        }

        public abstract Platform Platform { get; }

        public virtual IEnumerable<object> GetSyntheticCustomAttributes(string typeName) {
            return Enumerable.Empty<object>();
        }

        public virtual IEnumerable<object> GetSyntheticCustomAttributes(string typeName, string memberName) {
            return Enumerable.Empty<object>();
        }

        public override AttachedType? ProvideAttachedType(RawType rawType, RawType? companionRawType) {
            // TODO: Provide proper implementation
            return null;
        }

        /// <summary>
        /// Get the value of the specified attribute. The value considered the constructor argument with the specified name, if
        /// the attribute constructor has named arguments, or the first positional argument.
        /// </summary>
        /// <param name="attributes">attribute list</param>
        /// <param name="attributeTypeName"></param>
        /// <param name="argumentOptionalName">constructor arg name, with attribute value</param>
        /// <returns>attribute value or null if not found</returns>
        public static object? GetAttributeValue(IEnumerable<DotNetRawCustomAttribute> attributes,
            string attributeTypeName, string argumentOptionalName) {
            foreach (DotNetRawCustomAttribute attribute in attributes) {
                if (attribute.AttributeType.FullName != attributeTypeName)
                    continue;

                // First, see if the value was specified via a named argument
                object? namedArgumentValue = attribute.GetNamedArgumentValue(argumentOptionalName);
                if (namedArgumentValue != null)
                    return namedArgumentValue;

                if (attribute.GetPositionalArgumentCount() < 1)
                    return null;

                object value = attribute.GetPositionalArgumentValue(0);
                return value;
            }

            return null;
        }
    }
}
