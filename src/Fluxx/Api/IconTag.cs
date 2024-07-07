using Microsoft.CodeAnalysis.Text;

namespace Faml.Api {
    [Serializable]
    public struct IconTag {
        private readonly TextSpan _sourceSpan;
        private readonly IconTagType _iconTagType;

        public IconTag(TextSpan sourceSpan, IconTagType iconTagType) {
            _sourceSpan = sourceSpan;
            _iconTagType = iconTagType;
        }

        public TextSpan SourceSpan => _sourceSpan;

        public IconTagType IconTagType => _iconTagType;
    }
}
