using System;
using Microsoft.CodeAnalysis.Text;

namespace Fluxx.Api.IntelliSense
{
    [Serializable]
    public class IntelliSenseStartData
    {
        public TextSpan ApplicableToSpan { get; }

        public IntelliSenseStartData(TextSpan applicableToSpan)
        {
            this.ApplicableToSpan = applicableToSpan;
        }
    }
}
