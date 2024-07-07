using System;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Api.QuickInfo
{
    [Serializable]
    public class FunctionInvocationQuickInfo : QuickInfo
    {
        private readonly QualifiableName _functionName;
        private readonly QualifiableName? _returnTypeName;

        public FunctionInvocationQuickInfo(TextSpan textSpan, QualifiableName functionName, QualifiableName? returnTypeName) : base(textSpan)
        {
            this._functionName = functionName;
            this._returnTypeName = returnTypeName;
        }

        public QualifiableName FunctionName => this._functionName;

        public QualifiableName? ReturnTypeName => this._returnTypeName;
    }
}
