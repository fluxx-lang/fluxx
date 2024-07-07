using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Fluxx.VisualStudio.Taggers {
    [Export(typeof(ITaggerProvider))]
    [ContentType(FamlPackage.FamlContentType)]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    [TagType(typeof(SpaceNegotiatingAdornmentTag))]
    [TagType(typeof(IconSpaceNegotiatingTag))]
    internal sealed class IconTaggerProvider : ITaggerProvider {
        [Import]
        internal ITextDocumentFactoryService TextDocumentFactoryService = null;

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag {
            var famlModuleBuffer = FamlModuleBuffer.GetOrCreateFromTextBuffer(buffer);
            return (ITagger<T>) new IconTagger(famlModuleBuffer);
        }
    }

    /*
        /// <summary>
        /// Classifier provider. It adds the classifier to the set of classifiers.
        /// </summary>
        [Export(typeof(IClassifierProvider))]
        [FamlContentType(FamlPackagePrevious.FamlContentType)]   // This classifier applies to all FAML files.
        internal class FamlClassifierProvider : IClassifierProvider {
            /// <summary>
            /// Classification registry to be used for getting a reference
            /// to the custom classification type later.
            /// </summary>
            [Import]
            private IClassificationTypeRegistryService _classificationRegistry;

            /// <summary>
            /// Gets a classifier for the given text buffer.
            /// </summary>
            /// <param name="buffer">The <see cref="ITextBuffer"/> to classify.</param>
            /// <returns>A classifier for the text buffer, or null if the provider cannot do so in its current state.</returns>
            public IClassifier GetClassifier(ITextBuffer buffer) {
                return buffer.Properties.GetOrCreateSingletonProperty(() => new FamlClassifier(_classificationRegistry));
            }
        }
    */
}
