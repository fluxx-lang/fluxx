using System;
using Microsoft.CodeAnalysis.Text;

namespace Fluxx.Api
{
    [Serializable]
    public struct SyntaxHighlightTag
    {
        private readonly TextSpan sourceSpan;
        private readonly SyntaxHighlightTagType syntaxHighlightTagType;

        public SyntaxHighlightTag(TextSpan sourceSpan, SyntaxHighlightTagType syntaxHighlightTagType)
        {
            this.sourceSpan = sourceSpan;
            this.syntaxHighlightTagType = syntaxHighlightTagType;
        }

        public TextSpan SourceSpan => this.sourceSpan;

        public SyntaxHighlightTagType SyntaxHighlightTagType => this.syntaxHighlightTagType;
    }
}
