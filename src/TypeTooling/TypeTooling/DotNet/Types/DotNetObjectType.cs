using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TypeTooling.ClassifiedText;
using TypeTooling.CodeGeneration.Expressions;
using TypeTooling.CompanionType;
using TypeTooling.DotNet.CodeGeneration;
using TypeTooling.DotNet.RawTypes;
using TypeTooling.DotNet.RawTypes.Reflection;
using TypeTooling.Helper;
using TypeTooling.RawTypes;
using TypeTooling.Types;

namespace TypeTooling.DotNet.Types
{
    public class DotNetObjectType : ObjectTypeLazyLoaded
    {
        private readonly TypeToolingEnvironment _typeToolingEnvironment;
        private readonly DotNetRawType _rawType;
        private readonly DotNetRawType? _companionType;

        public DotNetObjectType(TypeToolingEnvironment typeToolingEnvironment, DotNetRawType rawType, DotNetRawType? companionType)
        {
            this._typeToolingEnvironment = typeToolingEnvironment;
            this._rawType = rawType;
            this._companionType = companionType;
        }

        public override RawType UnderlyingType => this._rawType;

        public TypeToolingEnvironment TypeToolingEnvironment => this._typeToolingEnvironment;

        public DotNetRawType RawType => this._rawType;

        public DotNetRawType? CompanionType => this._companionType;

        protected override ObjectTypeData DoGetData()
        {
            object? companionTypeObject = null;
            if (this._companionType != null)
            {
                companionTypeObject = this._typeToolingEnvironment.Instantiate(this._companionType);
            }

            var objectProperties = new List<ObjectProperty>();
            foreach (DotNetRawProperty platformProperty in this._rawType.GetPublicProperties())
            {
                TypeToolingType? type = this.GetPropertyType(platformProperty, companionTypeObject);

                // For now, skip properties with types we don't recognize
                if (type == null)
                {
                    continue;
                }

                objectProperties.Add(new DotNetObjectProperty(this, platformProperty, type));
            }

            CustomLiteralParser? customLiteralParser = this.GetObjectCustomLiteralParser(objectProperties, companionTypeObject);
            ObjectProperty? contentProperty = this.GetContentProperty(objectProperties, companionTypeObject);
            IReadOnlyCollection<ObjectType> baseTypes = this.GetBaseTypes(companionTypeObject);

            return new ObjectTypeData(fullName: this._rawType.FullName, properties: objectProperties, contentProperty: contentProperty,
                customLiteralParser: customLiteralParser, baseTypes: baseTypes);
        }

        /// <summary>
        /// Get the TypeToolingType for the specified property. By default, this method just maps the .NET type to the
        /// TypeToolingType, by calling back to the TypeToolingEnvironment. But a subclass can change this behavior if
        /// it has some extra knowledge about the property that indicates it should be a different type or have different
        /// behavior. For instance, if there's a TypeConvertor attribute on the property, then the returned type would
        /// normally have that CustomLiteralParser configured for the returned type.
        /// </summary>
        /// <param name="rawProperty">.NET PropertyInfo for the property</param>
        /// <param name="companionTypeObject">Companion object, for this object's type, if there is one</param>
        /// <returns>TypeToolingType to use for the property</returns>
        public virtual TypeToolingType? GetPropertyType(DotNetRawProperty rawProperty, object? companionTypeObject)
        {
            TypeToolingType? typeToolingType = this._typeToolingEnvironment.GetType(rawProperty.PropertyType);
            if (typeToolingType == null)
            {
                return null;
            }

            CustomLiteralParser? customLiteralParser = this.GetPropertyCustomLiteralParser(rawProperty, companionTypeObject);
            if (customLiteralParser != null)
            {
                if (!(typeToolingType is ObjectType objectType))
                {
                    // TODO: Support non-object custom literal types (like IsVisible boolean for Xamarin Forms)
                    return typeToolingType;
                }
#if false
                    throw new Exception(
                        $"Type ObjectTypes are currently supported for property specific custom literal creators, not type {typeToolingType.GetType().FullName} for {typeToolingType.FullName}");
#endif

                var enhancedObjectType = new EnhancedObjectType(objectType);
                enhancedObjectType.TypeOverride.OverrideCustomLiteralParser(customLiteralParser);

                return enhancedObjectType;
            }

            return typeToolingType;
        }

        private static object CreateCompanionType(DotNetRawType companionRawType)
        {
            foreach (DotNetRawConstructor constructor in companionRawType.GetConstructors())
            {
                if (constructor.GetParameters().Length == 0)
                {
                    ConstructorInfo constructorInfo = ((ReflectionDotNetRawConstructor) constructor).ConstructorInfo;
                    return constructorInfo.Invoke(null);
                }
            }

            throw new Exception($"No empty argument constructor found for companion type {companionRawType.FullName}");
        }

        protected virtual ObjectProperty? GetContentProperty(List<ObjectProperty> objectProperties, object? companionTypeObject)
        {
            ObjectProperty? contentProperty = this.GetContentPropertyFromCompanion(objectProperties, companionTypeObject, out bool explicitlyUnset);

            if (contentProperty != null || explicitlyUnset)
            {
                return contentProperty;
            }

            // If the current type doesn't specify a ContentProperty, check its base type
            return this.GetBaseDotNetObjectType()?.ContentProperty;
        }

        public DotNetObjectType? GetBaseDotNetObjectType()
        {
            DotNetRawType? baseRawType = this.RawType.BaseType;
            if (baseRawType == null)
            {
                return null;
            }

            return (DotNetObjectType?)this.TypeToolingEnvironment.GetType(baseRawType);
        }

        protected ObjectProperty? GetContentPropertyFromCompanion(List<ObjectProperty> objectProperties, object? companionTypeObject, out bool explicitlyUnset)
        {
            if (companionTypeObject == null ||
                !(companionTypeObject is IContentPropertyProvider contentPropertyProvider))
                {
                explicitlyUnset = false;
                return null;
            }

            string contentPropertyName = contentPropertyProvider.GetContentProperty();

            // The empty string means that there's explicitly no content property; it can be used to remove the content property set on a base class
            if (contentPropertyName.Length == 0)
            {
                explicitlyUnset = true;
                return null;
            }

            foreach (ObjectProperty property in objectProperties)
            {
                if (property.Name == contentPropertyName)
                {
                    explicitlyUnset = false;
                    return property;
                }
            }

            throw new Exception(
                $"Content property '{contentPropertyName}', specified by {companionTypeObject.GetType().FullName}, does not exist");
        }

        protected virtual CustomLiteralParser? GetObjectCustomLiteralParser(List<ObjectProperty> objectProperties, object? companionTypeObject)
        {
            if (companionTypeObject == null || !(companionTypeObject is ICustomLiteralParser customLiteralParser))
            {
                return null;
            }

            return new DotNetObjectCustomLiteralParser(customLiteralParser);
        }

        protected virtual CustomLiteralParser? GetPropertyCustomLiteralParser(DotNetRawProperty rawProperty,
            object? companionTypeObject)
            {
            // TODO: Provide companion support for property specific custom literal parsers
            return null;
        }

        protected virtual IReadOnlyCollection<ObjectType> GetBaseTypes(object? companionTypeObject)
        {
            var baseTypes = new List<ObjectType>();

            DotNetRawType? baseRawType = this._rawType.BaseType;
            if (baseRawType != null)
            {
                baseTypes.Add((ObjectType)this._typeToolingEnvironment.GetRequiredType(baseRawType));
            }

            foreach (DotNetRawType interfaceTypeDescriptor in this._rawType.GetInterfaces())
            {
                baseTypes.Add((ObjectType)this._typeToolingEnvironment.GetRequiredType(interfaceTypeDescriptor));
            }

            return baseTypes;
        }

        public override ExpressionCode GetCreateObjectCode(PropertyValue<string, ExpressionCode>[] propertyValues,
            PropertyValue<AttachedProperty, ExpressionCode>[] attachedPropertyValues)
            {
            DotNetRawConstructor? constructor = null;

            foreach (DotNetRawConstructor currConstructor in this._rawType.GetConstructors())
            {
                if (currConstructor.GetParameters().Length == 0)
                {
                    constructor = currConstructor;
                }
            }

            if (constructor == null)
            {
                throw new Exception($"No public empty argument constructor found for class: {this.FullName}");
            }

            // TODO: Beef this up some
            // Get the InitComplete method, if there is one; this call will return null if it doesn't exist
            DotNetRawMethod? initCompleteMethod = this._rawType.GetMethod("InitComplete", new DotNetRawType[] { });

            // TODO: Handle attached properties
            if (attachedPropertyValues.Length > 0)
            {
                throw new Exception("Attached properties aren't handled yet");
            }

#if false
            for (int i = 0; i < qualifiedPropertiesLength; i++) {
                QualifiableName propertyName = qualifiedPropertyNames[i];

                ObjectEval objectPropertyValue = CreateEvals.BoxIfPrimitiveType(qualifiedPropertyValues[i]);
                MethodInfo setterMethodInfo =
                    GetAttachedPropertySetterMethod(module, externalObjectTypeBinding, propertyName);
                propertyInitializers.Add(new ExternalPropertySetter(setterMethodInfo, objectPropertyValue));
            }
#endif

            return DotNetCode.New(this._rawType, propertyValues);

            /*
            _expressions = expressions;

            if (_expressions.Length > 0) {
                _childrenPropertyInfo = classTypeInfo.GetDeclaredProperty("Children");

                if (_childrenPropertyInfo == null)
                    throw new Exception("No 'Children' property found for class: " + classTypeInfo.Name);
            }
            */

            /*
            int expressionsLength = expressions.Length;
            _expressionAdders = new CSharpAdder[expressionsLength];
            for (int i = 0; i < expressionsLength; i++) {
                Eval expression = expressions[i];

                if (expression is ObjectEval) {
                    _expressionAdders[i] = new CSharpObjectAdder(classTypeInfo, (ObjectEval) expression, null /* TODO CONV Object.class #1#);
                } else throw new Exception("Unsupported expression type for " + expression);
            }
            */
        }

        public override GetPropertyCode GetGetPropertyCode(ExpressionCode instance, string property)
        {
            return DotNetCode.Property(this._rawType, instance, property);
        }

        public override InterpretedObjectCreator? GetInterpretedObjectCreator(ObjectProperty[] properties,
            AttachedProperty[] attachedProperties)
            {
            DotNetRawConstructor? constructor = null;

            foreach (DotNetRawConstructor currConstructor in this._rawType.GetConstructors())
            {
                if (currConstructor.GetParameters().Length == 0)
                {
                    constructor = currConstructor;
                }
            }

            if (constructor == null)
            {
                throw new Exception($"No public empty argument constructor found for class: {this.FullName}");
            }

            // TODO: Beef this up some
            // Get the InitComplete method, if there is one; this call will return null if it doesn't exist
            DotNetRawMethod? initCompleteMethod = this._rawType.GetMethod("InitComplete", new DotNetRawType[0] {});

            int propertiesLength = properties.Length;
            int attachedPropertiesLength = attachedProperties.Length;

            var propertyInitializers = new List<PropertyInitializer>();
            for (int i = 0; i < propertiesLength; i++)
            {
                if (!(properties[i] is DotNetObjectProperty dotNetObjectProperty))
                {
                    throw new Exception("Only properties of type DotNetObjectProperty can be set on DotNetObjectType objects");
                }

                propertyInitializers.Add(this.CreatePropertyInitializer(dotNetObjectProperty));
            }

            // TODO: Handle attached properties
            if (attachedPropertiesLength > 0)
            {
                throw new Exception("Attached properties aren't handled yet");
            }

#if false
            for (int i = 0; i < qualifiedPropertiesLength; i++) {
                QualifiableName propertyName = qualifiedPropertyNames[i];

                ObjectEval objectPropertyValue = CreateEvals.BoxIfPrimitiveType(qualifiedPropertyValues[i]);
                MethodInfo setterMethodInfo =
                    GetAttachedPropertySetterMethod(module, externalObjectTypeBinding, propertyName);
                propertyInitializers.Add(new ExternalPropertySetter(setterMethodInfo, objectPropertyValue));
            }
#endif

            return new DotNetInterpretedObjectCreator(constructor, propertyInitializers.ToArray());

            /*
            _expressions = expressions;

            if (_expressions.Length > 0) {
                _childrenPropertyInfo = classTypeInfo.GetDeclaredProperty("Children");

                if (_childrenPropertyInfo == null)
                    throw new Exception("No 'Children' property found for class: " + classTypeInfo.Name);
            }
            */

            /*
            int expressionsLength = expressions.Length;
            _expressionAdders = new CSharpAdder[expressionsLength];
            for (int i = 0; i < expressionsLength; i++) {
                Eval expression = expressions[i];

                if (expression is ObjectEval) {
                    _expressionAdders[i] = new CSharpObjectAdder(classTypeInfo, (ObjectEval) expression, null /* TODO CONV Object.class #1#);
                } else throw new Exception("Unsupported expression type for " + expression);
            }
            */
        }

        public override ObjectPropertyReader GetPropertyReader(ObjectProperty property)
        {
            if (!(property is DotNetObjectProperty dotNetObjectProperty))
            {
                throw new Exception("Only properties of type DotNetObjectProperty can be accessed on DotNetObjectType objects");
            }

            return new DotNetObjectPropertyReader(dotNetObjectProperty.RawProperty);
        }

        public override Task<ClassifiedTextMarkup?> GetDescriptionAsync(CancellationToken cancellationToken)
        {
            return this.RawType.GetDescriptionAsync(this._typeToolingEnvironment.UICulture, cancellationToken);
        }

        protected PropertyInitializer CreatePropertyInitializer(DotNetObjectProperty property)
        {
            /*
            if (propertyName.ToString() == "~self") {
                // TODO: Implement this
                MethodInfo listAdderMethod = null;
                propertyInitializer =
                    new ListObjectInitializer(classTypeInfo, (ListEval) propertyValue, listAdderMethod);
            }
            */

            DotNetRawProperty rawProperty = property.RawProperty;
            if (rawProperty == null)
            {
                throw new Exception($"Property '{property.Name}' doesn't exist for type '{this.FullName}'");
            }

            DotNetRawType propertyRawType = rawProperty.PropertyType;
            TypeToolingType propertyType = this._typeToolingEnvironment.GetRequiredType(propertyRawType);

            if (propertyType is DotNetSequenceType dotNetCollectionType)
            {
                return new CollectionPropertyInitializer(rawProperty, dotNetCollectionType.ElementRawType);
            }

            if (! rawProperty.CanWrite)
            {
                throw new Exception(
                    $"Property '{property.Name}' on type '{this.FullName}' isn't settable");
            }

            return new SimplePropertyInitializer(rawProperty);

#if false
            if (propertyInitializer == null) {
                EventInfo eventInfo = DotNetUtil.GetEventInfo(classTypeInfo, propertyName);

                if (eventInfo != null) {
                    var cSharpDataEventHandler = (DotNetDataEventHandler) module.Project.DataEventHandler;
                    if (cSharpDataEventHandler == null)
                        throw new Exception(
                            $"Event '{propertyName}' is assigned to, but no DataEventHandler is provided by the FAML driver");

                    ListEval listPropertyValue;
                    if (propertyValue is ListEval) {
                        listPropertyValue = (ListEval) propertyValue;
                    }
                    else if (propertyValue is ObjectEval) {
                        var listObjects = new ObjectEval[1];
                        listObjects[0] = (ObjectEval) propertyValue;
                        listPropertyValue = new ListEval(listObjects);
                    }
                    else throw new Exception($"Unsupported data type for event {propertyValue}");

                    propertyInitializer =
                        new DataEventHandlerInitializer(listPropertyValue, eventInfo, cSharpDataEventHandler);
                }
            }
#endif
        }
    }
}
