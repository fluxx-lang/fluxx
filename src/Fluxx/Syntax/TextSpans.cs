using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax
{
    public struct TextSpans
    {
        private readonly TextSpan[] _textSpans;
        

        public TextSpans(TextSpan[] textSpans)
        {
            this._textSpans = textSpans;
        }

        public bool OverlapsWith(TextSpan other)
        {
            foreach (TextSpan textSpan in this._textSpans)
            {
                if (textSpan.OverlapsWith(other))
                {
                    return true;
                }
            }

            return false;
        }

        public TextSpan[] Spans => this._textSpans;
    }
}
