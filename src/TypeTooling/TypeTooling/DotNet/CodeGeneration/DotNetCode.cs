using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using TypeTooling.CodeGeneration;
using TypeTooling.CodeGeneration.Expressions;
using TypeTooling.CodeGeneration.Expressions.Literals;
using TypeTooling.DotNet.RawTypes;
using TypeTooling.RawTypes;

namespace TypeTooling.DotNet.CodeGeneration
{
    public class DotNetCode
    {
        public static NewObjectCode New(DotNetRawType type, string[] parameterTypes, ExpressionCode[] constructorArguments,
            PropertyValue<string, ExpressionCode>[] propertyValues)
            {
            DotNetRawConstructor? rawConstructor = type.GetConstructor(parameterTypes);
            if (rawConstructor == null)
            {
                string desiredConstructor = GetConstructorDescription(type, parameterTypes);

                StringBuilder availableConstructors = new StringBuilder();
                foreach (DotNetRawConstructor constructor in type.GetConstructors())
                {
                    string description = GetConstructorDescription(type,
                        constructor.GetParameters().Select(parameterInfo => parameterInfo.ParameterType.FullName));
                    availableConstructors.Append(description);
                    availableConstructors.Append('\n');
                }

                throw new UserViewableException(
                    $"Specified public constructor not found for type {type.FullName}:\n{desiredConstructor}\n\nFound constructors:\n{availableConstructors}");
            }

            PropertyValue<RawProperty, ExpressionCode>[] newObjectPropertyValues = propertyValues.Select(propertyValue =>
                new PropertyValue<RawProperty, ExpressionCode>(GetRawProperty(type, propertyValue.Property), propertyValue.Value)).ToArray();

            return new NewObjectCode(type, rawConstructor, constructorArguments, newObjectPropertyValues);
        }

        private static DotNetRawProperty GetRawProperty(DotNetRawType type, string property)
        {
            DotNetRawProperty? rawProperty = type.GetProperty(property);
            if (rawProperty == null)
            {
                throw new Exception($"Property {property} doesn't exist for object type {type.FullName}");
            }

            return rawProperty;
        }

        public static NewObjectCode New(DotNetRawType type, string[] parameterTypes, params ExpressionCode[] constructorArguments)
        {
            return New(type, parameterTypes, constructorArguments, Array.Empty<PropertyValue<string, ExpressionCode>>());
        }

        public static NewObjectCode New(DotNetRawType type, params PropertyValue<string, ExpressionCode>[] propertyValues)
        {
            return New(type, Array.Empty<string>(), Array.Empty<ExpressionCode>(), propertyValues);
        }

        public static NewObjectCode New(DotNetRawType type)
        {
            return New(type, Array.Empty<string>(), Array.Empty<ExpressionCode>());
        }

        private static string GetConstructorDescription(DotNetRawType rawType, IEnumerable<string> parameterTypes)
        {
            return $"{rawType.Name}{GetParametersDescription(parameterTypes)}";
        }

        private static string GetParametersDescription(IEnumerable<string> paramaterTypes)
        {
            var builder = new StringBuilder("(");

            bool first = true;
            foreach (string parameterType in paramaterTypes)
            {
                if (!first)
                {
                    builder.Append(", ");
                }

                builder.Append(parameterType);

                first = false;
            }

            builder.Append(")");
            return builder.ToString();
        }

        public static MethodCallCode Call(DotNetRawType type, ExpressionCode instance, string methodName, DotNetRawType[] parameterRawTypes, params ExpressionCode[] arguments)
        {
            DotNetRawMethod method = type.GetRequiredMethod(methodName, parameterRawTypes);
            if (method.IsStatic)
            {
                throw new UserViewableException($"'{type.FullName}.{methodName}' method is static when expected not to be");
            }

            return new MethodCallCode(instance, method, arguments);
        }

        public static MethodCallCode Call(DotNetRawMethod method, ExpressionCode instance, params ExpressionCode[] arguments)
        {
            if (method.IsStatic)
            {
                throw new UserViewableException($"'{method.Name}' method is static when expected not to be");
            }

            return new MethodCallCode(instance, method, arguments);
        }

        public static MethodCallCode CallStatic(DotNetRawType type, string methodName, DotNetRawType[] parameterRawTypes, params ExpressionCode[] arguments)
        {
            DotNetRawMethod method = type.GetRequiredMethod(methodName, parameterRawTypes);
            if (!method.IsStatic)
            {
                throw new UserViewableException($"'{type.FullName}.{methodName}' method isn't static so it can't be called as a static method");
            }

            return new MethodCallCode(null, method, arguments);
        }

        public static MethodCallCode CallStatic(DotNetRawMethod method, params ExpressionCode[] arguments)
        {
            if (!method.IsStatic)
            {
                throw new UserViewableException($"'{method.Name}' method isn't static so it can't be called as a static method");
            }

            return new MethodCallCode(null, method, arguments);
        }

        public static GenericMethodCallCode CallGeneric(DotNetRawType[] genericTypeArguments, DotNetRawMethod method, ExpressionCode instance, params ExpressionCode[] arguments)
        {
            if (method.IsStatic)
            {
                throw new UserViewableException($"'{method.Name}' method is static when expected not to be");
            }

            return new GenericMethodCallCode(genericTypeArguments, instance, method, arguments);
        }

        public static GenericMethodCallCode CallStaticGeneric(DotNetRawType[] genericTypeArguments, DotNetRawMethod method, params ExpressionCode[] arguments)
        {
            if (!method.IsStatic)
            {
                throw new UserViewableException($"'{method.Name}' method isn't static so it can't be called as a static method");
            }

            return new GenericMethodCallCode(genericTypeArguments, null, method, arguments);
        }

        public static GetPropertyCode Property(DotNetRawType type, ExpressionCode? instance, DotNetRawProperty property)
        {
            return new GetPropertyCode(instance, property);
        }

        public static GetPropertyCode Property(DotNetRawType type, ExpressionCode? instance, string property)
        {
            return new GetPropertyCode(instance, GetRawProperty(type, property));
        }

        public static EnumValueCode EnumValue(DotNetRawType enumType, string enumValue)
        {
            // Verify that the value exists
            if (!(enumType.GetEnumNames().Any(name => name == enumValue)))
            {
                throw new UserViewableException($"No '{enumValue}' enum found for type '{enumType.FullName}'");
            }

            return new EnumValueCode(enumType, enumValue);
        }

        public static FunctionDelegateInvokeCode Invoke(FunctionDelegateHolder functionDelegateHolder, ImmutableArray<ExpressionCode> arguments)
        {
            return Code.Invoke(new FunctionDelegateHolderCode(functionDelegateHolder), arguments);
        }
    }
}
