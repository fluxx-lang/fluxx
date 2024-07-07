using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Fluxx.VisualStudio.Taggers {
    [Export(typeof(ITaggerProvider))]
    [ContentType(FamlPackage.FamlContentType)]
    [TagType(typeof(ClassificationTag))]
    internal sealed class SyntaxHighlightTaggerProvider : ITaggerProvider {
        [Import]
        internal IClassificationTypeRegistryService ClassificationTypeRegistry = null;

        [Import]
        internal ITextDocumentFactoryService TextDocumentFactoryService = null;

        [Import]
        internal IStandardClassificationService StandardClassificationService = null;

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag {
            try {
#if false
                if (!FamlPackagePrevious.Instance.IsSolutionReady())
                    return null;
#endif

                var moduleBuffer = FamlModuleBuffer.GetOrCreateFromTextBuffer(buffer);
                return (ITagger<T>) new SyntaxHighlightTagger(moduleBuffer, ClassificationTypeRegistry, StandardClassificationService);
            }
            catch (Exception e) {
                System.Diagnostics.Debug.WriteLine($"Error creating tagging provider: {e}");
                return null;
            }
        }
    }

    /*
        /// <summary>
        /// Classifier provider. It adds the classifier to the set of classifiers.
        /// </summary>
        [Export(typeof(IClassifierProvider))]
        [FamlContentType(FamlPackagePrevious.FamlContentType)]
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
