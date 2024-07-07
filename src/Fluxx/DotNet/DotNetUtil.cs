using System;
using System.Linq;
using System.Reflection;
using Fluxx.Api;

namespace Fluxx.DotNet
{
    public class DotNetUtil
    {
        public static PropertyInfo? GetPropertyInfo(TypeInfo typeInfo, Name propertyName)
        {
            string propertyNameString = propertyName.ToString();

            while (typeInfo != null)
            {
                PropertyInfo propertyInfo = typeInfo.GetDeclaredProperty(propertyNameString);
                if (propertyInfo != null)
                {
                    return propertyInfo;
                }

                typeInfo = typeInfo.BaseType?.GetTypeInfo();
            }

            return null;
        }

        public static EventInfo? GetEventInfo(TypeInfo typeInfo, Name eventName)
        {
            string eventNameString = eventName.ToString();

            while (typeInfo != null)
            {
                EventInfo eventInfo = typeInfo.GetDeclaredEvent(eventNameString);
                if (eventInfo != null)
                {
                    return eventInfo;
                }

                typeInfo = typeInfo.BaseType?.GetTypeInfo();
            }

            return null;
        }

        public static MethodInfo? GetMethodInfo(TypeInfo typeInfo, Name methodName)
        {
            string methodNameString = methodName.ToString();

            while (typeInfo != null)
            {
                MethodInfo methodInfo = typeInfo.GetDeclaredMethod(methodNameString);
                if (methodInfo != null)
                {
                    return methodInfo;
                }

                typeInfo = typeInfo.BaseType?.GetTypeInfo();
            }

            return null;
        }
        
        public static bool ImplementsIList(Type type)
        {
            if (IsIList(type))
            {
                return true;
            }

            return type.GetTypeInfo().ImplementedInterfaces.Any(IsIList);
        }

        private static bool IsIList(Type type)
        {
            string interfaceTypeName = type.FullName;
            return interfaceTypeName.Equals("System.Collections.IList") || interfaceTypeName.StartsWith("System.Collections.Generic.IList");
        }

        public static Attribute? GetSingleCustomAttribute(TypeInfo typeInfo, Type attributeType)
        {
            Attribute? singleAttribute = null;
            foreach (Attribute attribute in typeInfo.GetCustomAttributes(attributeType, true))
            {
                if (singleAttribute != null)
                {
                    throw new Exception($"Multiple {attributeType.Name} attributes defined when only one is allowed");
                }

                singleAttribute = attribute;
            }

            return singleAttribute;
        }
    }
}
