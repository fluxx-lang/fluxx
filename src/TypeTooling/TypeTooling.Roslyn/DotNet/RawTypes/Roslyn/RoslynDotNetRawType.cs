using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using TypeTooling.ClassifiedText;

namespace TypeTooling.DotNet.RawTypes.Roslyn {
    public class RoslynDotNetRawType : DotNetRawType {
        private readonly ITypeSymbol _typeSymbol;
        private readonly string _fullName;

        public RoslynDotNetRawType(ITypeSymbol typeSymbol) {
            _typeSymbol = typeSymbol;
            _fullName = _typeSymbol.GetFullName();
        }

        public ITypeSymbol TypeSymbol => _typeSymbol;

        public override string FullName => _fullName;

        public override string Name => _typeSymbol.Name;

        public override bool IsEnum => _typeSymbol.TypeKind == TypeKind.Enum;

        public override bool IsClass => _typeSymbol.TypeKind == TypeKind.Class;

        public override DotNetRawType? BaseType => ToRawType(_typeSymbol.BaseType);

        public override IEnumerable<DotNetRawType> GetInterfaces() {
            foreach (INamedTypeSymbol interfaceType in _typeSymbol.Interfaces)
                yield return new RoslynDotNetRawType(interfaceType);
        }

        public override IEnumerable<DotNetRawCustomAttribute> GetCustomAttributes() {
            foreach (AttributeData customAttributeData in _typeSymbol.GetAttributes())
                yield return new RoslynDotNetRawCustomAttribute(customAttributeData);
        }

        public override IEnumerable<DotNetRawConstructor> GetConstructors() {
            if (_typeSymbol is INamedTypeSymbol namedTypeSymbol) {
                foreach (IMethodSymbol constructor in namedTypeSymbol.InstanceConstructors)
                    yield return new RoslynDotNetRawConstructor(constructor);
            }
       }

        public override DotNetRawConstructor? GetConstructor(string[] parameterTypes) {
            if (_typeSymbol is INamedTypeSymbol namedTypeSymbol) {
                foreach (IMethodSymbol constructor in namedTypeSymbol.InstanceConstructors)
                    if (ParametersMatch(constructor, parameterTypes))
                        return new RoslynDotNetRawConstructor(constructor);
            }
            return null;
        }

        private bool ParametersMatch(IMethodSymbol methodSymbol, string[] parameterTypes) {
            ImmutableArray<IParameterSymbol> parameterSymbols = methodSymbol.Parameters;
            if (parameterSymbols.Length != parameterTypes.Length)
                return false;

            for (int i = 0; i < parameterSymbols.Length; i++) {
                if (parameterSymbols[i].Type.GetFullName() != parameterTypes[i])
                    return false;
            }

            return true;
        }

        private static ITypeSymbol[] ToTypeSymbols(DotNetRawType[] parameterRawTypes) {
            var parameterTypes = new ITypeSymbol[parameterRawTypes.Length];

            for (int i = 0; i < parameterRawTypes.Length; i++) {
                DotNetRawType parameterRawType = parameterRawTypes[i];

                if (!(parameterRawType is RoslynDotNetRawType roslynParameterType))
                    throw new Exception(
                        $"Parameter type is '{parameterRawType.FullName}', not a RoslynDotNetRawType as expected");

                parameterTypes[i] = roslynParameterType.TypeSymbol;
            }

            return parameterTypes;
        }

        public static RoslynDotNetRawType? ToRawType(INamedTypeSymbol? type) {
            return type == null ? null : new RoslynDotNetRawType(type);
        }

        public override IEnumerable<DotNetRawMethod> GetPublicMethods() {
            foreach (ISymbol member in _typeSymbol.GetMembers()) {
                if (member is IMethodSymbol method && method.DeclaredAccessibility == Accessibility.Public)
                    yield return new RoslynDotNetRawMethod(method);
            }
        }

        public override DotNetRawMethod? GetMethod(string methodName, DotNetRawType[] parameterRawTypes) {
            ITypeSymbol[] parameterTypes = ToTypeSymbols(parameterRawTypes);
            IMethodSymbol? methodSymbol = GetMethodWithSignature(_typeSymbol, methodName, parameterTypes);
            if (methodSymbol != null)
                return new RoslynDotNetRawMethod(methodSymbol);

            // TODO: Configure this is needed for Roslyn
            // If the method wasn't found, also search the interfaces for it. When an interface method is implemented with
            // the interface name included (e.g. "int MyInterface.Method() { return 3; }") then it needs to be retrieved this
            // way as it won't be found just looking at the type directly
            foreach (INamedTypeSymbol intface in _typeSymbol.Interfaces) {
                var intfaceMethodSymbol = GetMethodWithSignature(intface, methodName, parameterTypes);
                if (intfaceMethodSymbol != null)
                    return new RoslynDotNetRawMethod(intfaceMethodSymbol);
            }

            return null;
        }

        public override DotNetRawMethod? GetMethod(string methodName) {
            IMethodSymbol? methodSymbol = GetMethodWithName(_typeSymbol, methodName);
            if (methodSymbol != null)
                return new RoslynDotNetRawMethod(methodSymbol);

            // TODO: Configure this is needed for Roslyn
            // If the method wasn't found, also search the interfaces for it. When an interface method is implemented with
            // the interface name included (e.g. "int MyInterface.Method() { return 3; }") then it needs to be retrieved this
            // way as it won't be found just looking at the type directly
            foreach (INamedTypeSymbol intface in _typeSymbol.Interfaces) {
                var intfaceMethodSymbol = GetMethodWithName(intface, methodName);
                if (intfaceMethodSymbol != null)
                    return new RoslynDotNetRawMethod(intfaceMethodSymbol);
            }

            return null;
        }

        private static IMethodSymbol? GetMethodWithSignature(ITypeSymbol typeSymbol, string methodName, ITypeSymbol[] desiredParameterTypes) {
            foreach (ISymbol member in typeSymbol.GetMembers(methodName)) {
                if (member is IMethodSymbol method) {
                    ImmutableArray<IParameterSymbol> parameters = method.Parameters;

                    int parametersLength = parameters.Length;
                    if (parametersLength != desiredParameterTypes.Length)
                        continue;

                    for (int i = 0; i < parametersLength; i++) {
                        if (! parameters[i].Type.Equals(desiredParameterTypes[i]))
                            continue;
                    }

                    return method;
                }
            }

            return null;
        }

        /// <summary>
        /// Find a method by name only. If the method is overloaded, with multiple methods of that name taking different parameters,
        /// then null is returned.
        /// </summary>
        /// <param name="typeSymbol">type to search</param>
        /// <param name="methodName">method name</param>
        /// <returns>found method symbol or null</returns>
        private static IMethodSymbol? GetMethodWithName(ITypeSymbol typeSymbol, string methodName) {
            IMethodSymbol? foundMethod = null;

            foreach (ISymbol member in typeSymbol.GetMembers(methodName)) {
                if (member is IMethodSymbol method) {
                    if (foundMethod != null)
                        return null;
                    foundMethod = method;
                }
            }

            return foundMethod;
        }

        public override DotNetRawMethod? GetMethodInTypeOrAncestor(string methodName, DotNetRawType[] parameterRawTypes) {
            ITypeSymbol[] parameterTypes = ToTypeSymbols(parameterRawTypes);

            foreach (ITypeSymbol ancestorType in GetTypeAndAncestors(_typeSymbol)) {
                var methodSymbol = GetMethodWithSignature(ancestorType, methodName, parameterTypes);
                if (methodSymbol != null)
                    return new RoslynDotNetRawMethod(methodSymbol);
            }

            return null;
        }

        public override IEnumerable<DotNetRawProperty> GetPublicProperties() {
            foreach (ISymbol member in _typeSymbol.GetMembers())
                if (member is IPropertySymbol propertySymbol && propertySymbol.DeclaredAccessibility == Accessibility.Public)
                    yield return new RoslynDotNetRawProperty(propertySymbol);
        }

        public override DotNetRawProperty? GetProperty(string propertyName) {
            foreach (ISymbol member in _typeSymbol.GetMembers(propertyName))
                if (member is IPropertySymbol propertySymbol && propertySymbol.DeclaredAccessibility == Accessibility.Public)
                    return new RoslynDotNetRawProperty(propertySymbol);
            return null;
        }

        public override IEnumerable<DotNetRawField> GetPublicFields() {
            foreach (ISymbol member in _typeSymbol.GetMembers())
                if (member is IFieldSymbol fieldSymbol && fieldSymbol.DeclaredAccessibility == Accessibility.Public)
                    yield return new RoslynDotNetRawField(fieldSymbol);
        }

        public override IEnumerable<string> GetEnumNames() {
            // TODO: Is this right???
            foreach (var enumSymbol in _typeSymbol.GetMembers())
                yield return enumSymbol.Name;
        }

        public override object GetEnumUnderlyingValue(string enumName) {
            throw new NotImplementedException();
#if false
            INamedTypeSymbol underlyningType = _typeSymbol.EnumUnderlyingType;

            string[] enumNames = _typeSymbol.GetEnumNames();
            for (int i = 0; i < enumNames.Length; i++) {
                if (enumName == enumNames[i]) {
                    Array enumValues = _typeSymbol.GetEnumValues();
                    object value = enumValues.GetValue(i);
                    return System.Convert.ChangeType(value, underlyningType);
                }
            }

            throw new UserViewableException($"Enum {_typeSymbol.FullName} doesn't have a value named {enumName}");
#endif
        }

        public override bool IsAssignableFrom(DotNetRawType otherType) {
            var otherTypeSymbol = ((RoslynDotNetRawType) otherType).TypeSymbol;

            if (_typeSymbol.Equals(otherTypeSymbol))
                return true;

            TypeKind myTypeKind = _typeSymbol.TypeKind;
            if (myTypeKind == TypeKind.Class)
                return HasAncestorClass(otherTypeSymbol, _typeSymbol);
            else if (myTypeKind == TypeKind.Interface)
                return HasAncestorInterface(otherTypeSymbol, _typeSymbol);
            else return false;
        }

        private static bool HasAncestorClass(ITypeSymbol typeSymbol, ITypeSymbol potentialAncestor) {
            ITypeSymbol ancestorClass = typeSymbol.BaseType;
            while (ancestorClass != null) {
                if (ancestorClass.Equals(potentialAncestor))
                    return true;
                ancestorClass = ancestorClass.BaseType;
            }

            return false;
        }

        private static bool HasAncestorInterface(ITypeSymbol typeSymbol, ITypeSymbol potentialAncestor) {
            foreach (INamedTypeSymbol parentInterface in typeSymbol.Interfaces) {
                if (parentInterface.Equals(potentialAncestor))
                    return true;
                if (HasAncestorInterface(parentInterface, potentialAncestor))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Return the current type and all ancestor types for it, searching the base class and interfaces in a depth first search.
        /// Note that interface types can be returned multiple times here, if there are multiple parts of the hiearchy that
        /// derive from them.
        /// </summary>
        /// <param name="typeSymbol">type in question</param>
        /// <returns>current type and all its ancestor classes and interfaces</returns>
        private static IEnumerable<ITypeSymbol> GetTypeAndAncestors(ITypeSymbol typeSymbol) {
            // Return current type
            yield return typeSymbol;

            // Return superclass and its ancestors
            INamedTypeSymbol baseType = typeSymbol.BaseType;
            if (baseType != null)
                foreach (ITypeSymbol ancestorType in GetTypeAndAncestors(baseType))
                    yield return ancestorType;

            // TODO: Do we really need to return interfaces here, given how we use this?
            // Return interfaces and their ancestors
            foreach (INamedTypeSymbol intface in typeSymbol.Interfaces)
                foreach (ITypeSymbol ancestorType in GetTypeAndAncestors(intface))
                    yield return ancestorType;
        }

        /// <summary>
        /// See if this class has the specified type as a superclass. Note that only classes are checked here, not implemented
        /// interfaces. If the specified type is the same as the current type, it's considered to be a superclass.
        /// </summary>
        /// <param name="otherType">type in question</param>
        /// <returns>true if the specified type is a superclass of this class</returns>
        public override bool HasAncestorType(DotNetRawType otherType) {
            ITypeSymbol potentialSuperclass = ((RoslynDotNetRawType) otherType)._typeSymbol;

            ITypeSymbol currentTypeSymbol = _typeSymbol;
            while (currentTypeSymbol != null) {
                if (currentTypeSymbol.Equals(potentialSuperclass))
                    return true;
                currentTypeSymbol = currentTypeSymbol.BaseType;
            }

            return false;
        }

        public override DotNetRawType? GetEnumerableElementType() {
            Type iEnumerableType = typeof(IEnumerable<>);
            string iEnumerableTypeName = iEnumerableType.FullName;

            foreach (INamedTypeSymbol interfaceType in _typeSymbol.Interfaces) {
                if (interfaceType.IsGenericType && interfaceType.MetadataName == iEnumerableTypeName) {
                    ITypeSymbol elementTypeSymbol = interfaceType.TypeArguments[0];
                    if (!(elementTypeSymbol is INamedTypeSymbol elementNamedTypeSymbol))
                        return null;
                    return new RoslynDotNetRawType(elementNamedTypeSymbol);
                }
            }

            return null;
        }

        public override async Task<ClassifiedTextMarkup?> GetDescriptionAsync(CultureInfo preferredCulture,
            CancellationToken cancellationToken) {

            ClassifiedTextMarkup? descriptionMarkup = await Task.Run(() => {
                string? documentCommentXml = _typeSymbol.GetDocumentationCommentXml(preferredCulture, cancellationToken: cancellationToken);
                if (documentCommentXml == null)
                    return null;

                return DescriptionMarkupCreator.CreateDescriptionMarkup(documentCommentXml);
            }, cancellationToken);

            return descriptionMarkup;
        }

        public override bool Equals(object obj) {
            return obj is RoslynDotNetRawType reflectionDotNetRawType && _typeSymbol.Equals(reflectionDotNetRawType._typeSymbol);
        }

        public override int GetHashCode() {
            return _typeSymbol.GetHashCode();
        }
    }
}
