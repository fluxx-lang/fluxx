using System;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Api.QuickInfo
{
    [Serializable]
    public abstract class QuickInfo
    {
        private readonly TextSpan _textSpan;

        protected QuickInfo(TextSpan textSpan)
        {
            this._textSpan = textSpan;
        }

        public TextSpan Span => this._textSpan;
    }
}
