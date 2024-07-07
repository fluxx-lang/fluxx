using System;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Api.IntelliSense {
    [Serializable]
    public class IntelliSenseStartData {
        public TextSpan ApplicableToSpan { get; }

        public IntelliSenseStartData(TextSpan applicableToSpan) {
            this.ApplicableToSpan = applicableToSpan;
        }
    }
}
