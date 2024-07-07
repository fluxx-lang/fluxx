using System;
using Microsoft.CodeAnalysis.Text;

namespace Fluxx.Api.QuickInfo
{
    [Serializable]
    public abstract class QuickInfo
    {
        private readonly TextSpan textSpan;

        protected QuickInfo(TextSpan textSpan)
        {
            this.textSpan = textSpan;
        }

        public TextSpan Span => this.textSpan;
    }
}
