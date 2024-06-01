using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using TypeTooling.ClassifiedText;
using TypeTooling.RawTypes;

namespace TypeTooling.DotNet.RawTypes {
    public abstract class DotNetRawType : RawType {
        public abstract string FullName { get; }

        public abstract string Name { get; }

        public override string ToString() {
            return FullName;
        }

        public abstract bool IsEnum { get; }

        public abstract bool IsClass { get; }

        public abstract DotNetRawType? BaseType { get; }

        public abstract IEnumerable<DotNetRawType> GetInterfaces();

        public abstract IEnumerable<DotNetRawCustomAttribute> GetCustomAttributes();

        public bool HasAttributeOfType(string attributeTypeFullName) {
            foreach (DotNetRawCustomAttribute attribute in GetCustomAttributes()) {
                if (attribute.AttributeType.FullName == attributeTypeFullName)
                    return true;
            }

            return false;
        }

        public abstract IEnumerable<DotNetRawConstructor> GetConstructors();

        public abstract DotNetRawConstructor? GetConstructor(string[] parameterTypes);

        public abstract IEnumerable<DotNetRawMethod> GetPublicMethods();

        public abstract DotNetRawMethod? GetMethod(string methodName, DotNetRawType[] parameterRawTypes);

        public DotNetRawMethod GetRequiredMethod(string methodName, DotNetRawType[] parameterRawTypes) {
            DotNetRawMethod? method = GetMethod(methodName, parameterRawTypes);
            if (method == null)
                throw new UserViewableException($"No '{methodName}' method found for type '{FullName}' taking specified parameters");
            return method;
        }

        public abstract DotNetRawMethod? GetMethod(string methodName);

        public DotNetRawMethod GetRequiredMethod(string methodName) {
            DotNetRawMethod? method = GetMethod(methodName);
            if (method == null)
                throw new UserViewableException($"No '{methodName}' method found for type '{FullName}'");
            return method;
        }

        public abstract DotNetRawMethod? GetMethodInTypeOrAncestor(string methodName, DotNetRawType[] methodArgs);

        public DotNetRawMethod GetRequiredInstanceMethod(string methodName, DotNetRawType[] parameterRawTypes) {
            DotNetRawMethod? method = GetMethod(methodName, parameterRawTypes);
            if (method == null)
                throw new UserViewableException($"No '{methodName}' method found for type '{FullName}'");
            if (method.IsStatic)
                throw new UserViewableException($"'{FullName}.{methodName}' method is static when expected not to be");
            return method;
        }

        public DotNetRawMethod GetRequiredStaticMethod(string methodName, DotNetRawType[] parameterRawTypes) {
            DotNetRawMethod? method = GetMethod(methodName, parameterRawTypes);
            if (method == null)
                throw new UserViewableException($"No '{methodName}' method found for type '{FullName}'");
            if (!method.IsStatic)
                throw new UserViewableException($"'{FullName}.{methodName}' method is not static when expected it to be");
            return method;
        }

        public abstract IEnumerable<DotNetRawProperty> GetPublicProperties();

        public abstract DotNetRawProperty? GetProperty(string propertyName);

        public abstract IEnumerable<DotNetRawField> GetPublicFields();

        public abstract IEnumerable<string> GetEnumNames();

        public abstract object GetEnumUnderlyingValue(string enumName);

        public abstract bool IsAssignableFrom(DotNetRawType otherType);

        public bool IsAssignableTo(DotNetRawType otherType) {
            return otherType.IsAssignableFrom(this);
        }

        /// <summary>
        /// See if this class has the specified type as a superclass. Note that only classes are checked here, not implemented
        /// interfaces. If the specified type is the same as the current type, it's considered to be a superclass.
        /// </summary>
        /// <param name="otherType">type in question</param>
        /// <returns>true if the specified type is a superclass of this class</returns>
        public abstract bool HasAncestorType(DotNetRawType otherType);

        /// <summary>
        /// If the type implemented IEnumerable, then return the element type for the enumerable (the generic argument).
        /// Otherwise, return null.
        /// </summary>
        /// <returns>IEnumerable element type or null</returns>
        public abstract DotNetRawType? GetEnumerableElementType();

        public abstract Task<ClassifiedTextMarkup?> GetDescriptionAsync(CultureInfo preferredCulture,
            CancellationToken cancellationToken);
    }
}
