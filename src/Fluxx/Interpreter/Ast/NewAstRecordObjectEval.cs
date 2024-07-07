using System;
using Fluxx.Api;
using Fluxx.Interpreter.Ast;

namespace Fluxx.Interpreter.record
{
    public sealed class NewAstRecordObjectEval : ObjectEval
    {
        private readonly AstRecordPropertySetter[] propertySetters;

        public NewAstRecordObjectEval(Name[] propertyNames, Eval[] propertyValues)
        {
            int propertiesLength = propertyNames.Length;
            this.propertySetters = new AstRecordPropertySetter[propertiesLength];
            for (int i = 0; i < propertiesLength; i++)
            {
                string propertyNameString = propertyNames[i].ToString();
                Eval propertyValue = propertyValues[i];

                AstRecordPropertySetter astRecordPropertySetter;
                if (propertyValue is IntEval)
                {
                    astRecordPropertySetter = new AstRecordPropertyIntSetter(propertyNameString, (IntEval)propertyValue);
                }
                else if (propertyValue is BooleanEval)
                {
                    astRecordPropertySetter = new AstRecordPropertyBooleanSetter(propertyNameString, (BooleanEval)propertyValue);
                }
                else if (propertyValue is ObjectEval)
                {
                    astRecordPropertySetter = new AstRecordPropertyObjectSetter(propertyNameString, (ObjectEval)propertyValue);
                }

                /*
                else if (propertyValue is ListEval) {
                    recordPropertySetter = new RecordPropertyIntSetter(propertyName, (ListEval)propertyValue);
                }
                */
                else
                {
                    throw new Exception($"Unsupported property type for '{propertyValue}'");
                }

                this.propertySetters[i] = astRecordPropertySetter;
            }
        }

        public override object Eval()
        {
            var record = new AstRecord();

            int length = this.propertySetters.Length;
            for (int i = 0; i < length; i++)
            {
                this.propertySetters[i].Invoke(record);
            }

            return record;
        }
    }
}
