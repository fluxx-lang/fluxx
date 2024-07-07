using System;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Api
{
    [Serializable]
    public struct SyntaxHighlightTag
    {
        private readonly TextSpan _sourceSpan;
        private readonly SyntaxHighlightTagType _syntaxHighlightTagType;

        public SyntaxHighlightTag(TextSpan sourceSpan, SyntaxHighlightTagType syntaxHighlightTagType)
        {
            this._sourceSpan = sourceSpan;
            this._syntaxHighlightTagType = syntaxHighlightTagType;
        }

        public TextSpan SourceSpan => this._sourceSpan;

        public SyntaxHighlightTagType SyntaxHighlightTagType => this._syntaxHighlightTagType;
    }
}
