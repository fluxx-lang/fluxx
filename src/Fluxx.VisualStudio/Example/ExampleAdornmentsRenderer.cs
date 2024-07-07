using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Fluxx.DevEnv;
using Fluxx.Lang;
using Fluxx.Syntax;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using ReactiveData;

namespace Fluxx.VisualStudio.Example {
    internal sealed class ExampleAdornmentsRenderer {
        /// <summary>
        /// The layer of the adornment
        /// </summary>
        private readonly IAdornmentLayer _layer;

        /// <summary>
        /// Text view where the adornment is created
        /// </summary>
        private readonly IWpfTextView _view;

        private readonly FamlModuleBuffer _famlModuleBuffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExampleAdornmentRenderer"/> class.
        /// </summary>
        /// <param name="view">Text view to create the adornment for</param>
        public ExampleAdornmentsRenderer(IWpfTextView view) {
            _famlModuleBuffer = FamlModuleBuffer.GetOrCreateFromTextBuffer(view.TextBuffer);

            _layer = view.GetAdornmentLayer("ExampleAdornments");

            _view = view;
            _view.LayoutChanged += this.OnLayoutChanged;
        }

        /// <summary>
        /// Handles whenever the text displayed in the view changes by adding the adornment to any reformatted lines
        /// </summary>
        /// <remarks><para>This event is raised whenever the rendered text displayed in the <see cref="ITextView"/> changes.</para>
        /// <para>It is raised whenever the view does a layout (which happens when DisplayTextLineContainingBufferPosition is called or in response to text or classification changes).</para>
        /// <para>It is also raised whenever the view scrolls horizontally or when its size changes.</para>
        /// </remarks>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        internal void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e) {
            AddExampleAdornments();
        }

        private void AddExampleAdornments() {
            // Remove any previous adornments
            _layer.RemoveAllAdornments();

            FamlModule module = _famlModuleBuffer.FamlModule;
            if (module == null)
                return;

            FamlProject project = _famlModuleBuffer.Project;
            if (project == null || project.AnyErrors)
                return;

            ExampleDefinitionSyntax[] examples = module.ModuleSyntax.ExampleDefinitions;
            ExampleManager exampleManager = _famlModuleBuffer.ExampleManager;
            for (int exampleIndex = 0; exampleIndex < examples.Length; exampleIndex++) {
                ExampleDefinitionSyntax example = examples[exampleIndex];

                int exampleEndSourcePosition = example.Span.End;

                ITextSnapshotLine textSnapshotLine = _view.TextSnapshot.GetLineFromPosition(exampleEndSourcePosition);

                // TODO: Only draw adornment if visible.  Use IWpfTextViewLineCollection.First(Last)VisibleLine 

                IWpfTextViewLineCollection textViewLines = _view.TextViewLines;
                Geometry lineGeometry = textViewLines.GetMarkerGeometry(textSnapshotLine.Extent);
                if (lineGeometry == null)
                    continue;

                RenderedExample renderedExample = exampleManager.Get(exampleIndex);
                if (renderedExample == null) {
                    //FamlExecutionContext executionContext = new LocalExecutionContext(projectProxy);
                    AppExecutionContext executionContext = new AppExecutionContext(_famlModuleBuffer.FamlVisualStudioWorkspace.AppConnection);
                    IReactive<ExampleResult[]> exampleResults = executionContext.VisualizeExample(module.ModuleName, exampleIndex);

                    renderedExample = exampleManager.Add(exampleIndex, exampleResults, SynchronizationContext.Current,
                        () => { _famlModuleBuffer.NotifyTagsChanged(new SnapshotSpanEventArgs(textSnapshotLine.Extent)); });
                }

                if (!renderedExample.IsPresentationAvailable)
                    renderedExample = exampleManager.GetPrevious(exampleIndex);

                if (renderedExample != null && renderedExample.IsPresentationAvailable)
                    AddExampleAdornment(textSnapshotLine, lineGeometry, renderedExample.UiElement);
            }
        }

        private void AddExampleAdornment(ITextSnapshotLine textSnapshotLine, Geometry lineGeometry, UIElement uiElement) {
            // TODO: Only draw adornment if visible.  Use IWpfTextViewLineCollection.First(Last)VisibleLine 

            // Position the example value below the example, left aligned with it
            Canvas.SetLeft(uiElement, lineGeometry.Bounds.Left);
            Canvas.SetTop(uiElement, lineGeometry.Bounds.Bottom);

            _layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, textSnapshotLine.Extent, null, uiElement, null);
        }
    }
}
