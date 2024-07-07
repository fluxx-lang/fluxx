using Microsoft.CodeAnalysis.Text;

namespace Faml.Api {
    [Serializable]
    public struct IconTag {
        private readonly TextSpan _sourceSpan;
        private readonly IconTagType _iconTagType;

        public IconTag(TextSpan sourceSpan, IconTagType iconTagType) {
            this._sourceSpan = sourceSpan;
            this._iconTagType = iconTagType;
        }

        public TextSpan SourceSpan => this._sourceSpan;

        public IconTagType IconTagType => this._iconTagType;
    }
}
