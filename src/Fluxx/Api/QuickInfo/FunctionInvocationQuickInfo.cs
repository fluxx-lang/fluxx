using System;
using Microsoft.CodeAnalysis.Text;

namespace Fluxx.Api.QuickInfo
{
    [Serializable]
    public class FunctionInvocationQuickInfo : QuickInfo
    {
        private readonly QualifiableName functionName;
        private readonly QualifiableName? returnTypeName;

        public FunctionInvocationQuickInfo(TextSpan textSpan, QualifiableName functionName, QualifiableName? returnTypeName) : base(textSpan)
        {
            this.functionName = functionName;
            this.returnTypeName = returnTypeName;
        }

        public QualifiableName FunctionName => this.functionName;

        public QualifiableName? ReturnTypeName => this.returnTypeName;
    }
}
