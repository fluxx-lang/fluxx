using System;
using Microsoft.CodeAnalysisP.Text;

namespace Faml.Api.QuickInfo {
    [Serializable]
    public abstract class QuickInfo {
        private readonly TextSpan _textSpan;

        protected QuickInfo(TextSpan textSpan) {
            _textSpan = textSpan;
        }

        public TextSpan Span => _textSpan;
    }
}
