using Microsoft.CodeAnalysis.Text;

namespace Fluxx.Syntax
{
    public struct TextSpans
    {
        private readonly TextSpan[] textSpans;

        public TextSpans(TextSpan[] textSpans)
        {
            this.textSpans = textSpans;
        }

        public bool OverlapsWith(TextSpan other)
        {
            foreach (TextSpan textSpan in this.textSpans)
            {
                if (textSpan.OverlapsWith(other))
                {
                    return true;
                }
            }

            return false;
        }

        public TextSpan[] Spans => this.textSpans;
    }
}
