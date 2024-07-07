using Microsoft.CodeAnalysis.Text;

namespace Faml.Api
{
    [Serializable]
    public struct IconTag
    {
        private readonly TextSpan sourceSpan;
        private readonly IconTagType iconTagType;

        public IconTag(TextSpan sourceSpan, IconTagType iconTagType)
        {
            this.sourceSpan = sourceSpan;
            this.iconTagType = iconTagType;
        }

        public TextSpan SourceSpan => this.sourceSpan;

        public IconTagType IconTagType => this.iconTagType;
    }
}
