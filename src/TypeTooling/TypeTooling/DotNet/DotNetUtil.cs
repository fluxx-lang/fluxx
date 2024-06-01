using System;
using System.Collections.Generic;
using System.Reflection;

namespace TypeTooling.DotNet {
    public class DotNetUtil {
        /// <summary>
        /// Return the current type and all ancestor types for it, searching the base class and interfaces in a depth first search.
        /// Note that interface types can be returned multiple times here, if there are multiple parts of the hiearchy that
        /// derive from them.
        /// </summary>
        /// <param name="type">type in question</param>
        /// <returns>current type and all its ancestor classes and interfaces</returns>
        public static IEnumerable<Type> GetTypeAndAncestors(Type type) {
            // Return current type
            yield return type;

            // Return superclass and its ancestors
            Type baseType = type.BaseType;
            if (baseType != null)
                foreach (Type ancestorType in GetTypeAndAncestors(baseType))
                    yield return ancestorType;

            // Return interfaces and their ancestors
            foreach (Type intface in type.GetInterfaces())
                foreach (Type ancestorType in GetTypeAndAncestors(intface))
                    yield return ancestorType;
        }

        public static MethodInfo? GetMethod(Type type, string methodName, Type[] methodArgs) {
            foreach (Type ancestorType in GetTypeAndAncestors(type)) {
                MethodInfo methodInfo = ancestorType.GetMethod(methodName, methodArgs);
                if (methodInfo != null)
                    return methodInfo;
            }

            return null;
        }
    }
}
