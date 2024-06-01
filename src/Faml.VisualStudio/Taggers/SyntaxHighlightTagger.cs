using System;
using System.Collections.Generic;
using System.Linq;
using Faml.Api;
using Faml.Syntax;
using Microsoft.CodeAnalysisP.Text;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace Faml.VisualStudio.Taggers {
    /// <summary>
    /// Classifier that classifies all text as an instance of the "FamlClassifier" classification type.
    /// </summary>
    internal class SyntaxHighlightTagger : ITagger<ClassificationTag> {
        private readonly FamlModuleBuffer _famlModuleBuffer;
        private readonly Dictionary<SyntaxHighlightTagType, ClassificationTag> _classificationTags;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxHighlightTagger"/> class.
        /// </summary>
        /// <param name="famlModuleBuffer">Source buffer</param>
        /// <param name="standardClassificationService">Standard classification service</param>
        internal SyntaxHighlightTagger(FamlModuleBuffer famlModuleBuffer, IClassificationTypeRegistryService registry,
            IStandardClassificationService standardClassificationService) {
            _famlModuleBuffer = famlModuleBuffer;

            _classificationTags = new Dictionary<SyntaxHighlightTagType, ClassificationTag> {
                [SyntaxHighlightTagType.Keyword] = new ClassificationTag(standardClassificationService.Keyword),
                [SyntaxHighlightTagType.ControlKeyword] = CreateClassificationTag(registry, "keyword - control"),
                [SyntaxHighlightTagType.Comment] = new ClassificationTag(standardClassificationService.Comment),
                [SyntaxHighlightTagType.Punctuation] = CreateClassificationTag(registry, "punctuation"),
                [SyntaxHighlightTagType.DeemphasizedPunctuation] = CreateClassificationTag(registry, ClassificationTypeNames.FamlDeemphasizedPunctuation),
                [SyntaxHighlightTagType.Operator] = new ClassificationTag(standardClassificationService.Operator),
                [SyntaxHighlightTagType.FunctionReference] = CreateClassificationTag(registry, ClassificationTypeNames.FamlFunctionReference),
                [SyntaxHighlightTagType.PropertyReference] = CreateClassificationTag(registry, ClassificationTypeNames.FamlPropertyReference),
                [SyntaxHighlightTagType.TypeReference] = CreateClassificationTag(registry, "type"),
                [SyntaxHighlightTagType.SymbolReference] = new ClassificationTag(standardClassificationService.Identifier),
                [SyntaxHighlightTagType.NumberLiteral] = new ClassificationTag(standardClassificationService.NumberLiteral),
                [SyntaxHighlightTagType.NumberLiteral] = new ClassificationTag(standardClassificationService.NumberLiteral),
                [SyntaxHighlightTagType.StringLiteral] = CreateClassificationTag(registry, "string"),
                [SyntaxHighlightTagType.UIText] = CreateClassificationTag(registry, ClassificationTypeNames.FamlUIText),
                [SyntaxHighlightTagType.PropertyValue] = CreateClassificationTag(registry, ClassificationTypeNames.FamlPropertyValue),
                [SyntaxHighlightTagType.InvalidToken] = new ClassificationTag(standardClassificationService.Identifier),
            };

            famlModuleBuffer.TagsChanged += (sender, args) => OnTagsChanged(args);
        }

        private ClassificationTag CreateClassificationTag(IClassificationTypeRegistryService registry, string type) {
            IClassificationType classificationType = registry.GetClassificationType(type);
            if (classificationType == null)
                throw new ArgumentException($"No classification type found for type name {type}");

            return new ClassificationTag(classificationType);
        }

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
        /// <param name="span">The span currently being classified.</param>
        /// <returns>A list of ClassificationSpans that represent spans identified to be of this classification.</returns>
        public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
            FamlModule module = _famlModuleBuffer.FamlModule;
            if (module == null)
                yield break;
            else {
                ITextSnapshot snapshot = _famlModuleBuffer.TextBuffer.CurrentSnapshot;

                TextSpan[] textSpans = spans.Select(SnapshotSpanExtensions.ToTextSpan).ToArray();

                SyntaxHighlightTag[] syntaxHighlightTags = module.GetSyntaxHighlightTags(textSpans);
                foreach (SyntaxHighlightTag sourceTag in syntaxHighlightTags) {
                    var snapshotSpan = new SnapshotSpan(snapshot, sourceTag.SourceSpan.Start, sourceTag.SourceSpan.Length);
                    ClassificationTag classificationTag = _classificationTags[sourceTag.SyntaxHighlightTagType];

                    yield return new TagSpan<ClassificationTag>(snapshotSpan, classificationTag);
                }
            }

            /*
            if (classifySource.moduleProxy != null) {
                updateDevice(luxSource);
            }
            */
        }

/*
        private bool spanIntersectsAstNode(SnapshotSpan currSpan, SyntaxNode moduleProxy)
        {
            throw new NotImplementedException();
        }
*/

}
}
