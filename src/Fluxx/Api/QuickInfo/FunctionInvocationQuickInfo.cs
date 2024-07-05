using System;
using Microsoft.CodeAnalysisP.Text;

namespace Faml.Api.QuickInfo {
    [Serializable]
    public class FunctionInvocationQuickInfo : QuickInfo {
        private readonly QualifiableName _functionName;
        private readonly QualifiableName? _returnTypeName;

        public FunctionInvocationQuickInfo(TextSpan textSpan, QualifiableName functionName, QualifiableName? returnTypeName) : base(textSpan) {
            _functionName = functionName;
            _returnTypeName = returnTypeName;
        }

        public QualifiableName FunctionName => _functionName;

        public QualifiableName? ReturnTypeName => _returnTypeName;
    }
}
