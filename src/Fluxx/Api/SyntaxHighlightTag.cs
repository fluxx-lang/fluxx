using System;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Api {
    [Serializable]
    public struct SyntaxHighlightTag {
        private readonly TextSpan _sourceSpan;
        private readonly SyntaxHighlightTagType _syntaxHighlightTagType;

        public SyntaxHighlightTag(TextSpan sourceSpan, SyntaxHighlightTagType syntaxHighlightTagType) {
            _sourceSpan = sourceSpan;
            _syntaxHighlightTagType = syntaxHighlightTagType;
        }

        public TextSpan SourceSpan => _sourceSpan;

        public SyntaxHighlightTagType SyntaxHighlightTagType => _syntaxHighlightTagType;
    }
}
