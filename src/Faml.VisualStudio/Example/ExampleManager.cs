using System.Collections.Generic;
using System.Threading;
using Faml.Lang;
using Faml.Syntax;
using Microsoft.VisualStudio.Text.Editor;
using ReactiveData;

namespace Faml.VisualStudio.Example
{
    public class ExampleManager {
        private readonly FamlModuleBuffer _famlModuleBuffer;
        private int _activeExampleIndex = -1;
        private readonly Dictionary<int, RenderedExample> _renderedExamples = new Dictionary<int, RenderedExample>();
        private readonly Dictionary<int, RenderedExample> _previousRenderedExamples = new Dictionary<int, RenderedExample>();

        public ExampleManager(FamlModuleBuffer famlModuleBuffer) {
            _famlModuleBuffer = famlModuleBuffer;
        }

        public RenderedExample? Get(int exampleIndex) {
            return _renderedExamples.TryGetValue(exampleIndex, out RenderedExample renderedExample)
                ? renderedExample : null;
        }

        public RenderedExample GetPrevious(int exampleIndex) {
            return _previousRenderedExamples.TryGetValue(exampleIndex, out RenderedExample renderedExample)
                ? renderedExample : null;
        }

        public RenderedExample Add(int exampleIndex, IReactive<ExampleResult[]> exampleResults,
            SynchronizationContext uiSynchronizationContext,
            PresentationAvailableEventHandler presentationAvailableEventHandler) {
            var renderedExample = new RenderedExample(this, exampleIndex, exampleResults, uiSynchronizationContext,
                presentationAvailableEventHandler);
            _renderedExamples.Add(exampleIndex, renderedExample);

            return renderedExample;
        }

        internal void UpdatePreviousRenderedExamples(RenderedExample renderedExample) {
            _previousRenderedExamples[renderedExample.ExampleIndex] = renderedExample;
        }

        public void Clear() {
            _renderedExamples.Clear();
        }

        public void OnCaretPositionChanged(IWpfTextView textView, CaretPositionChangedEventArgs args) {
            int caretPosition = args.NewPosition.BufferPosition.Position;

            ExampleDefinitionSyntax exampleDefinition =
                _famlModuleBuffer.FamlModule?.ModuleSyntax.GetExampleDefinitionAtSourcePosition(caretPosition);

            if (exampleDefinition != null && exampleDefinition.ExampleIndex != _activeExampleIndex) {
                _famlModuleBuffer.VisualizeExample(exampleDefinition.ExampleIndex);
                _activeExampleIndex = exampleDefinition.ExampleIndex;
            }
        }
    }
}
