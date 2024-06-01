using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;

namespace Faml.VisualStudio.QuickInfo {
	[Export (typeof (IAsyncQuickInfoSourceProvider))]
    [Name("FAML FamlQuickInfoItem Provider")]
	[ContentType(FamlPackage.FamlContentType)]
    [Order]
    public class QuickInfoSourceProvider : IAsyncQuickInfoSourceProvider {
	    [Import] internal ITextDocumentFactoryService TextDocumentFactoryService = null;

        public IAsyncQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer) {
            FamlModuleBuffer famlModuleBuffer = FamlModuleBuffer.GetOrCreateFromTextBuffer(textBuffer);
            return new QuickInfoSource(famlModuleBuffer);
        }
    }
}
