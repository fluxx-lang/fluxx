using Microsoft.CodeAnalysisP.Text;
using Microsoft.VisualStudio.Text;

namespace Fluxx.VisualStudio {
    public static class SnapshotSpanExtensions {
        public static TextSpan ToTextSpan(this SnapshotSpan snapshotSpan) {
            return TextSpan.FromBounds(snapshotSpan.Start, snapshotSpan.End);
        }
    }
}