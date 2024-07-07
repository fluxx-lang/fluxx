using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax {
    public struct TextSpans {
        private readonly TextSpan[] _textSpans;
        

        public TextSpans(TextSpan[] textSpans) {
            _textSpans = textSpans;
        }

        public bool OverlapsWith(TextSpan other) {
            foreach (TextSpan textSpan in _textSpans) {
                if (textSpan.OverlapsWith(other))
                    return true;
            }

            return false;
        }

        public TextSpan[] Spans => _textSpans;
    }
}
