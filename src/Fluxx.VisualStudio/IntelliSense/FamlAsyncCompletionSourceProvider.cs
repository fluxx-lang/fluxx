using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace Fluxx.VisualStudio.IntelliSense {
    [Export(typeof(IAsyncCompletionSourceProvider))]
    [ContentType(FamlPackage.FamlContentType)]
    [ExportMetadata("ContentType", FamlPackage.FamlContentType)]
    [Name("FAML Completion Source Provider")]
    public class FamlAsyncCompletionSourceProvider : IAsyncCompletionSourceProvider {
        public IAsyncCompletionSource GetOrCreate(ITextView textView) {
            ThreadHelper.ThrowIfNotOnUIThread();

            FamlModuleBuffer famlModuleBuffer = FamlModuleBuffer.GetOrCreateFromTextBuffer(textView.TextBuffer);
            return new FamlAsyncCompletionSource(famlModuleBuffer);
        }
    }
}
