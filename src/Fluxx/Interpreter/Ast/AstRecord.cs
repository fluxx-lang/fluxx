using System;
using System.Collections.Generic;

namespace Faml.Interpreter.Ast
{
    public class AstRecord
    {
        private readonly Dictionary<string, object> _propertyValues = new Dictionary<string, object>();

        public AstRecord() {}

        public void SetProperty(string name, object value)
        {
            this._propertyValues[name] = value;
        }

        public object GetProperty(string name)
        {
            if (!this._propertyValues.TryGetValue(name, out object value))
            {
                throw new Exception($"Property '{name}' not set on data record");
            }

            return value;
       }
    }
}
