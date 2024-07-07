using System;
using System.Collections.Generic;
using Faml.Api;
using Faml.Binding.External;
using Faml.Syntax;
using TypeTooling.Types;

/**
 * @author Bret Johnson
 * @since 4/4/2015
 */
namespace Faml.Interpreter.External {
    public sealed class NewExternalObjectEval : ObjectEval {
        private readonly ObjectEval[] _propertyValueEvals;
        private readonly InterpretedObjectCreator _objectCreator;

        public NewExternalObjectEval(ModuleSyntax module, NewExternalObjectFunctionBinding functionBinding, Name[] propertyNames, Eval[] propertyValues,
            QualifiableName[] qualifiedPropertyNames, Eval[] qualifiedPropertyValues) {
            int propertiesLength = propertyNames.Length;
            int qualifiedPropertiesLength = qualifiedPropertyNames.Length;

            Dictionary<string, ObjectProperty> objectProperties = functionBinding.ReturnExternalObjectTypeBinding.ObjectProperties;

            var initializationObjectProperties = new List<ObjectProperty>();

            this._propertyValueEvals = new ObjectEval[propertiesLength];

            for (int i = 0; i < propertiesLength; i++) {
                // TODO: Fix this up to cast primitive types
                this._propertyValueEvals[i] = CreateObjectEval(propertyValues[i], propertyNames[i]);

                ObjectProperty objectProperty = objectProperties[propertyNames[i].ToString()];
                initializationObjectProperties.Add(objectProperty);
            }

            ObjectType objectType = functionBinding.ReturnExternalObjectTypeBinding.TypeToolingType;
            this._objectCreator = objectType.GetInterpretedObjectCreator(initializationObjectProperties.ToArray(), new AttachedProperty[0]);
        }

        private static ObjectEval CreateObjectEval(Eval propertyValue, Name propertyName) {
            if (propertyValue is IntEval intEval) {
                return new CastIntObjectEval(intEval);
                // TODO: Fix up this type conversion stuff
#if false
                if (propertyType.Equals("System.Byte"))
                    objectPropertyValue = new CastByteObjectEval((IntEval) propertyValue);
                else objectPropertyValue = new CastIntObjectEval((IntEval) propertyValue);
#endif
            }
            else if (propertyValue is BooleanEval booleanEval) {
                return new CastBooleanObjectEval(booleanEval);
            }
            else if (propertyValue is ObjectEval objectEval) {
                return objectEval;
            }
            else throw new Exception($"Unsupported property type for property {propertyName}: {propertyValue.GetType()}");
        }

        public override object Eval() {
            int propertiesLength = this._propertyValueEvals.Length;

            int startOffset = Context.StackIndex;

            for (int i = 0; i < propertiesLength; i++)
            {
                this._propertyValueEvals[i].Push();
            }

            object returnValue = this._objectCreator.Create(Context.ObjectStack, startOffset);

            Context.StackIndex -= propertiesLength;

            return returnValue;
        }
    }
}
