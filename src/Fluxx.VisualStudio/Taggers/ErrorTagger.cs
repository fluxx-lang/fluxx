using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysisP.Text;
using Faml.CodeAnalysis;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace Faml.VisualStudio.Taggers
{

    /// <summary>
    /// Classifier that classifies all text as an instance of the "FamlClassifier" classification type.
    /// </summary>
    internal class ErrorTagger : ITagger<ErrorTag> {
        private readonly FamlModuleBuffer _famlBuffer;

        /// <summary>
        /// An event that occurs when the classification of a span of text has changed.
        /// </summary>
        /// <remarks>
        /// This event gets raised if a non-text change would affect the classification in some way,
        /// for example typing /* would cause the classification to change in C# without directly
        /// affecting the span.
        /// </remarks>
        //public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxHighlightTagger"/> class.
        /// </summary>
        /// <param name="famlBuffer">Source buffer</param>
        internal ErrorTagger(FamlModuleBuffer famlBuffer) {
            _famlBuffer = famlBuffer;

            famlBuffer.TagsChanged += (sender, args) => OnTagsChanged(args);
        }

        private void OnTagsChanged(SnapshotSpanEventArgs e) {
            if (TagsChanged != null)
                TagsChanged(this, e);
        }

        /// <summary>
        /// Gets all the <see cref="ClassificationSpan"/> objects that intersect with the given range of text.
        /// </summary>
        /// <remarks>
        /// This method scans the given SnapshotSpan for potential matches for this classification.
        /// In this instance, it classifies everything and returns each span as a new ClassificationSpan.
        /// </remarks>
        /// <param name="spans">The span currently being classified.</param>
        /// <returns>A list of ClassificationSpans that represent spans identified to be of this classification.</returns>
        public IEnumerable<ITagSpan<ErrorTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
            FamlProject project = _famlBuffer.Project;

            if (project != null) {
                foreach (Diagnostic diagnostic in project.GetModuleDiagnostics(_famlBuffer.ModuleName)) {
                    //if (problem.modulePath.Equals(currModulePath))

                    TextSpan sourceSpan = diagnostic.SourceSpan;

                    // Ensure that the snapshotSpan doesn't go past the end of the text buffer, or else we'll get an ArgumentOutOfBounds exception
                    var snapshotSpan = new SnapshotSpan(_famlBuffer.TextBuffer.CurrentSnapshot, sourceSpan.Start,
                        Math.Min(sourceSpan.Length, _famlBuffer.TextBuffer.CurrentSnapshot.Length) );

                    if (spans.IntersectsWith(snapshotSpan))
                        yield return new TagSpan<ErrorTag>(snapshotSpan, new ErrorTag("luxError", diagnostic.Message));
                }
            }
        }
    }
}
