using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TypeTooling.ClassifiedText;
using TypeTooling.Types.PredefinedTypes;

namespace TypeTooling.DotNet.RawTypes.Reflection
{
    public class ReflectionDotNetRawType : DotNetRawType
    {
        public Type Type { get; }

        public static ReflectionDotNetRawType ForPredefinedType(PredefinedType predefinedType)
        {
            if (predefinedType is BooleanType)
            {
                return new ReflectionDotNetRawType(typeof(bool));
            }
            else if (predefinedType is DoubleType)
            {
                return new ReflectionDotNetRawType(typeof(double));
            }
            else if (predefinedType is IntegerType)
            {
                return new ReflectionDotNetRawType(typeof(int));
            }
            else if (predefinedType is StringType)
            {
                return new ReflectionDotNetRawType(typeof(string));
            }
            else
            {
                throw new InvalidOperationException($"Unknown predefined type: {predefinedType.GetType().FullName}");
            }
        }

        public ReflectionDotNetRawType(Type type)
        {
            this.Type = type;
        }

        public override string FullName => this.Type.FullName;

        public override string Name => this.Type.Name;

        public override bool IsEnum => this.Type.IsEnum;

        public override bool IsClass => this.Type.IsClass;

        public override DotNetRawType? BaseType => ToRawType(this.Type.BaseType);

        public override IEnumerable<DotNetRawType> GetInterfaces()
        {
            foreach (Type interfaceType in this.Type.GetInterfaces())
            {
                yield return new ReflectionDotNetRawType(interfaceType);
            }
        }

        public override IEnumerable<DotNetRawCustomAttribute> GetCustomAttributes()
        {
            foreach (CustomAttributeData customAttributeData in this.Type.CustomAttributes)
            {
                yield return new ReflectionDotNetRawCustomAttribute(customAttributeData);
            }
        }

        public override IEnumerable<DotNetRawConstructor> GetConstructors()
        {
            foreach (ConstructorInfo constructorInfo in this.Type.GetConstructors())
            {
                yield return new ReflectionDotNetRawConstructor(constructorInfo);
            }
        }

        public override DotNetRawConstructor? GetConstructor(string[] parameterTypes)
        {
            foreach (ConstructorInfo constructorInfo in this.Type.GetConstructors())
            {
                if (this.ParametersMatch(constructorInfo, parameterTypes))
                {
                    return new ReflectionDotNetRawConstructor(constructorInfo);
                }
            }

            return null;
        }

        private bool ParametersMatch(MethodBase methodBase, string[] parameterTypes)
        {
            ParameterInfo[] parameterInfos = methodBase.GetParameters();
            if (parameterInfos.Length != parameterTypes.Length)
            {
                return false;
            }

            for (int i = 0; i < parameterInfos.Length; i++)
            {
                if (parameterInfos[i].ParameterType.FullName != parameterTypes[i])
                {
                    return false;
                }
            }

            return true;
        }

        private static Type[] ToTypes(DotNetRawType[] parameterRawTypes)
        {
            var parameterTypes = new Type[parameterRawTypes.Length];

            for (int i = 0; i < parameterRawTypes.Length; i++)
            {
                DotNetRawType parameterRawType = parameterRawTypes[i];

                if (!(parameterRawType is ReflectionDotNetRawType reflectionParameterType))
                {
                    throw new Exception(
                        $"Parameter type is '{parameterRawType.FullName}', not a ReflectionDotNetType as expected");
                }

                parameterTypes[i] = reflectionParameterType.Type;
            }

            return parameterTypes;
        }

        public static ReflectionDotNetRawType? ToRawType(Type? type)
        {
            return type == null ? null : new ReflectionDotNetRawType(type);
        }

        public override IEnumerable<DotNetRawMethod> GetPublicMethods()
        {
            foreach (MethodInfo methodInfo in this.Type.GetMethods())
            {
                yield return new ReflectionDotNetRawMethod(methodInfo);
            }
        }

        public override DotNetRawMethod? GetMethod(string methodName, DotNetRawType[] parameterRawTypes)
        {
            Type[] parameterTypes = ToTypes(parameterRawTypes);
            MethodInfo methodInfo = this.Type.GetMethod(methodName, parameterTypes);
            if (methodInfo != null)
            {
                return new ReflectionDotNetRawMethod(methodInfo);
            }

            // If the method wasn't found, also search the interfaces for it. When an interface method is implemented with
            // the interface name included (e.g. "int MyInterface.Method() { return 3; }") then it needs to be retrieved this
            // way as it won't be found just looking at the type directly
            foreach (Type intface in this.Type.GetInterfaces())
            {
                MethodInfo intfaceMethodInfo = intface.GetMethod(methodName, parameterTypes);
                if (intfaceMethodInfo != null)
                {
                    return new ReflectionDotNetRawMethod(intfaceMethodInfo);
                }
            }

            return null;
        }

        /// <summary>
        /// Get a method by name only. If the name is overloaded, with multiple methods that only differ in their parameters,
        /// then null is returned.
        /// </summary>
        /// <param name="methodName">name of desired method</param>
        /// <returns></returns>
        public override DotNetRawMethod? GetMethod(string methodName)
        {
            try
            {
                MethodInfo methodInfo = this.Type.GetMethod(methodName);
                if (methodInfo != null)
                {
                    return new ReflectionDotNetRawMethod(methodInfo);
                }

                // If the method wasn't found, also search the interfaces for it. When an interface method is implemented with
                // the interface name included (e.g. "int MyInterface.Method() { return 3; }") then it needs to be retrieved this
                // way as it won't be found just looking at the type directly
                foreach (Type intface in this.Type.GetInterfaces())
                {
                    MethodInfo intfaceMethodInfo = intface.GetMethod(methodName);
                    if (intfaceMethodInfo != null)
                    {
                        return new ReflectionDotNetRawMethod(intfaceMethodInfo);
                    }
                }

                return null;
            }
            catch (AmbiguousMatchException e)
            {
                return null;
            }
        }

        public override DotNetRawMethod? GetMethodInTypeOrAncestor(string methodName, DotNetRawType[] parameterRawTypes)
        {
            Type[] parameterTypes = ToTypes(parameterRawTypes);

            foreach (Type ancestorType in GetTypeAndAncestors(this.Type))
            {
                MethodInfo methodInfo = ancestorType.GetMethod(methodName, parameterTypes);
                if (methodInfo != null)
                {
                    return new ReflectionDotNetRawMethod(methodInfo);
                }
            }

            return null;
        }

        public override IEnumerable<DotNetRawProperty> GetPublicProperties()
        {
            foreach (PropertyInfo propertyInfo in this.Type.GetProperties())
            {
                yield return new ReflectionDotNetRawProperty(propertyInfo);
            }
        }

        public override DotNetRawProperty? GetProperty(string propertyName)
        {
            PropertyInfo propertyInfo = this.Type.GetProperty(propertyName);
            if (propertyInfo == null)
            {
                return null;
            }

            return new ReflectionDotNetRawProperty(propertyInfo);
        }

        public override IEnumerable<DotNetRawField> GetPublicFields()
        {
            foreach (FieldInfo fieldInfo in this.Type.GetFields())
            {
                yield return new ReflectionDotNetRawField(fieldInfo);
            }
        }

        public override IEnumerable<string> GetEnumNames()
        {
            foreach (string enumName in this.Type.GetEnumNames())
            {
                yield return enumName;
            }
        }

        public override object GetEnumUnderlyingValue(string enumName)
        {
            Type underlyingType = System.Enum.GetUnderlyingType(this.Type);

            string[] enumNames = this.Type.GetEnumNames();
            for (int i = 0; i < enumNames.Length; i++)
            {
                if (enumName == enumNames[i])
                {
                    Array enumValues = this.Type.GetEnumValues();
                    object value = enumValues.GetValue(i);
                    return System.Convert.ChangeType(value, underlyingType);
                }
            }

            throw new UserViewableException($"Enum {this.Type.FullName} doesn't have a value named {enumName}");
        }

        public override bool IsAssignableFrom(DotNetRawType otherType)
        {
            return this.Type.IsAssignableFrom(((ReflectionDotNetRawType) otherType).Type);
        }

        /// <summary>
        /// Return the current type and all ancestor types for it, searching the base class and interfaces in a depth first search.
        /// Note that interface types can be returned multiple times here, if there are multiple parts of the hiearchy that
        /// derive from them.
        /// </summary>
        /// <param name="type">type in question</param>
        /// <returns>current type and all its ancestor classes and interfaces</returns>
        private static IEnumerable<Type> GetTypeAndAncestors(Type type)
        {
            // Return current type
            yield return type;

            // Return superclass and its ancestors
            Type baseType = type.BaseType;
            if (baseType != null)
            {
                foreach (Type ancestorType in GetTypeAndAncestors(baseType))
                {
                    yield return ancestorType;
                }
            }

            // Return interfaces and their ancestors
            foreach (Type intface in type.GetInterfaces())
            {
                foreach (Type ancestorType in GetTypeAndAncestors(intface))
                {
                    yield return ancestorType;
                }
            }
        }

        /// <summary>
        /// See if this class has the specified type as a superclass. Note that only classes are checked here, not implemented
        /// interfaces. If the specified type is the same as the current type, it's considered to be a superclass.
        /// </summary>
        /// <param name="otherType">type in question</param>
        /// <returns>true if the specified type is a superclass of this class</returns>
        public override bool HasAncestorType(DotNetRawType otherType)
        {
            Type potentialSuperclass = ((ReflectionDotNetRawType) otherType).Type;

            Type currentType = this.Type;
            while (currentType != null)
            {
                if (currentType == potentialSuperclass)
                {
                    return true;
                }

                currentType = currentType.BaseType;
            }

            return false;
        }

        public override DotNetRawType? GetEnumerableElementType()
        {
            Type iEnumerableType = typeof(IEnumerable<>);

            foreach (Type interfaceType in this.Type.GetInterfaces())
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == iEnumerableType)
                {
                    return new ReflectionDotNetRawType(interfaceType.GetGenericArguments()[0]);
                }
            }

            return null;
        }

        public override Task<ClassifiedTextMarkup?> GetDescriptionAsync(CultureInfo preferredCulture,
            CancellationToken cancellationToken)
            {
            return Task.FromResult((ClassifiedTextMarkup?)null);
        }

        public override bool Equals(object obj)
        {
            return obj is ReflectionDotNetRawType reflectionDotNetRawType &&
                   EqualityComparer<Type>.Default.Equals(this.Type, reflectionDotNetRawType.Type);
        }

        public override int GetHashCode()
        {
            return this.Type.GetHashCode();
        }
    }
}
