using System;
using System.Collections.Generic;
using System.Linq;
using Faml.Api;
using Faml.Syntax;
using Microsoft.CodeAnalysisP.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace Faml.VisualStudio.Taggers {
    /// <summary>
    /// Classifier that classifies all text as an instance of the "FamlClassifier" classification type.
    /// </summary>
    internal class IconTagger : ITagger<IconSpaceNegotiatingTag> {
        private readonly FamlModuleBuffer _famlBuffer;

        internal IconTagger(FamlModuleBuffer famlBuffer) {
            _famlBuffer = famlBuffer;

            famlBuffer.TagsChanged += (sender, args) => OnTagsChanged(args);
        }

        /// <summary>
        /// An event that occurs when the classification of a span of text has changed.
        /// </summary>
        /// <remarks>
        /// This event gets raised if a non-text change would affect the classification in some way,
        /// for example typing /* would cause the classification to change in C# without directly
        /// affecting the span.
        /// </remarks>
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        private void OnTagsChanged(SnapshotSpanEventArgs e) {
            TagsChanged?.Invoke(this, e);
        }

        public IEnumerable<ITagSpan<IconSpaceNegotiatingTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
            FamlModule? module = _famlBuffer.FamlModule;
            if (module == null)
                yield break;
            else {
                ITextSnapshot snapshot = _famlBuffer.TextBuffer.CurrentSnapshot;

                TextSpan[] textSpans = spans.Select(SnapshotSpanExtensions.ToTextSpan).ToArray();

                IconTag[] iconTags = module.GetIconTags(textSpans);
                foreach (IconTag iconTag in iconTags) {
                    var snapshotSpan = new SnapshotSpan(snapshot, iconTag.SourceSpan.Start, iconTag.SourceSpan.Length);
                    yield return new TagSpan<IconSpaceNegotiatingTag>(snapshotSpan, new IconSpaceNegotiatingTag(iconTag));
                }
            }

            /*
            if (classifySource.moduleProxy != null) {
                updateDevice(luxSource);
            }
            */
        }
    }
}
