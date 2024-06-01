using System;
using Microsoft.CodeAnalysisP.Text;

namespace Faml.Api.IntelliSense {
    [Serializable]
    public class IntelliSenseStartData {
        public TextSpan ApplicableToSpan { get; }

        public IntelliSenseStartData(TextSpan applicableToSpan) {
            ApplicableToSpan = applicableToSpan;
        }
    }
}
