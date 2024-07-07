using System.ComponentModel.Composition;
using Fluxx.VisualStudio.Example;
using Fluxx.VisualStudio.IconAdornments;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace Fluxx.VisualStudio {
    /// <summary>
    /// Establishes an <see cref="IAdornmentLayer"/> to place the adornment on and exports the <see cref="IWpfTextViewCreationListener"/>
    /// that instantiates the adornment on the event of a <see cref="IWpfTextView"/>'s creation
    /// </summary>
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType(FamlPackage.FamlContentType)]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal sealed class TextViewCreationListener : IWpfTextViewCreationListener {
        /// <summary>
        /// Defines the adornment layer for the adornment. This layer is ordered after the selection layer in the Z-order
        /// </summary>
        [Export(typeof(AdornmentLayerDefinition))]
        [Name("ExampleAdornments")]
        [Order(After = PredefinedAdornmentLayers.Selection, Before = PredefinedAdornmentLayers.Text)]
        private AdornmentLayerDefinition _exampleAdornments;

        /// <summary>
        /// Defines the adornment layer for the adornment. This layer is ordered after the selection layer in the Z-order
        /// </summary>
        [Export(typeof(AdornmentLayerDefinition))]
        [Name("IconAdornments")]
        [Order(After = "ExampleAdornments")]
        private AdornmentLayerDefinition _functionIconAdornments;

        [Import]
        internal ITextDocumentFactoryService TextDocumentFactoryService = null;

        /// <summary>
        /// Called when a text view having matching roles is created over a text data model having a matching content type.
        /// Instantiates a ExampleAdornment manager when the textView is created.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/> upon which the adornment should be placed</param>
        public void TextViewCreated(IWpfTextView textView) {
            // The adornment renderers will listen to any event that changes the layout (text changes, scrolling, etc)
            new ExampleAdornmentsRenderer(textView);
            new IconAdornmentRenderer(textView, TextDocumentFactoryService);

            FamlModuleBuffer famlModuleBuffer = FamlModuleBuffer.GetOrCreateFromTextBuffer(textView.TextBuffer);
            textView.Caret.PositionChanged += (sender, caretPositionChangedEventArgs) => {
                famlModuleBuffer.ExampleManager.OnCaretPositionChanged(textView, caretPositionChangedEventArgs);
            };
        }
    }
}
