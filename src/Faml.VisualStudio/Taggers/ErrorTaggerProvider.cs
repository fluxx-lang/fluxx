using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Faml.VisualStudio.Taggers {
    [Export(typeof(ITaggerProvider))]
    [ContentType(FamlPackage.FamlContentType)]
    [TagType(typeof(ErrorTag))]
    internal sealed class ErrorTaggerProvider : ITaggerProvider {
        [Import]
        internal ITextDocumentFactoryService TextDocumentFactoryService = null;

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag {
            var famlModuleBuffer = FamlModuleBuffer.GetOrCreateFromTextBuffer(buffer);
            return (ITagger<T>) new ErrorTagger(famlModuleBuffer);
        }
    }
}
