using Fluxx.Api;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace Fluxx.VisualStudio.Taggers {
    public class IconSpaceNegotiatingTag : SpaceNegotiatingAdornmentTag {
        public static string IconProviderTag = "IconProviderTag";

        public IconSpaceNegotiatingTag(IconTag iconTag) : base(16.0, 0, 16.0, 16.0, 0.0, PositionAffinity.Successor, iconTag, IconProviderTag) {
        }
    }
}