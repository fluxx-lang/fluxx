using System;
using System.Threading;
using System.Threading.Tasks;
using Fluxx.Api;
using Fluxx.Api.QuickInfo;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;

namespace Fluxx.VisualStudio.QuickInfo {
    internal class QuickInfoSource : IAsyncQuickInfoSource {
        private readonly FamlModuleBuffer _famlModuleBuffer;

        public QuickInfoSource(FamlModuleBuffer famlModuleBuffer) {
            this._famlModuleBuffer = famlModuleBuffer;
        }

        public void Dispose() {
        }

        public async Task<QuickInfoItem?> GetQuickInfoItemAsync(IAsyncQuickInfoSession session, CancellationToken cancellationToken) {
            try {
                return GetQuickInfoItem(session);
            }
            catch (Exception e) {
                await Logger.LogErrorAsync("GetQuickInfoItemAsync failed", e);
                return null;
            }

#if false
            elements.Clear();
            elements.Add(new ClassifiedTextElement(new ClassifiedTextRun(
                PredefinedClassificationTypeNames.Identifier, "MY FUNCTION")));

            await Microsoft.VisualStudio.Shell.ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var colorWell = new System.Windows.Shapes.Rectangle {
                Height = 16,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                Fill = new SolidColorBrush(Colors.CadetBlue),
                Margin = new System.Windows.Thickness(0, 1, 0, 3),
                RadiusX = 2,
                RadiusY = 2,
                SnapsToDevicePixels = true,
            };
            elements.Add(colorWell);

            var content = new ContainerElement(ContainerElementStyle.Stacked, elements);

            //var range = new SnapshotSpan(snapshot, famlQuickInfoItem.Span.Start, famlQuickInfoItem.Span.Length);
            var range = new SnapshotSpan(snapshot, 1, 2000);
            ITrackingSpan trackingSpan = snapshot.CreateTrackingSpan(range, SpanTrackingMode.EdgeInclusive);
            return new QuickInfoItem(trackingSpan, content);
#endif

#if false
            var parser = XmlParserService.GetConfiguredParser(triggerPoint.Value);
            if (parser == null)
                return null;

            var engine = _textBuffer.GetDesignerSession()?.LayoutCompletionEngine;
            if (engine == null)
                return null;

            if (!(parser.CurrentState is XmlAttributeValueState))
                return null;

            var attribute = parser.Nodes.Peek() as XAttribute;
            if (attribute == null)
                return null;

            // If the attribute still has no value it means we the offset we gave is in the middle
            // of it so continue parsing until we get the full value
            if (attribute.Value == null) {
                while (parser.CurrentState is XmlAttributeValueState)
                    parser.Push(snapshot[offset++]);
                // One more time to get a valid end region for the attribute
                parser.Push(snapshot[offset]);
                offset -= 1;
            }

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var render = await RenderResourceValue(engine, attribute.Value);
            if (render == null)
                return null;

            var startOffset = offset - attribute.Value.Length - 1;
            var range = new SnapshotSpan(snapshot, startOffset, offset - startOffset);
            var trackingSpan = snapshot.CreateTrackingSpan(range, SpanTrackingMode.EdgeInclusive);

            IEnumerable<string> path = render.Item2;
            object renderedValue = render.Item1;

            var elements = new List<object>();
            if (renderedValue is Xwt.Drawing.Color color) {
                var colorWell = new System.Windows.Shapes.Rectangle {
                    Height = 16,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                    Fill = new SolidColorBrush(color.ToWpfColor()),
                    Margin = new System.Windows.Thickness(0, 1, 0, 3),
                    RadiusX = 2,
                    RadiusY = 2,
                    SnapsToDevicePixels = true,
                };
                elements.Add(colorWell);
            }
            else if (renderedValue is Xwt.Widget widget)
                elements.Add(Xwt.Toolkit.CurrentEngine.GetNativeWidget(widget));
            // build path
            elements.AddRange(path.Select((e, i) => {
                if (i == 0)
                    return new ClassifiedTextElement(new ClassifiedTextRun(PredefinedClassificationTypeNames.MarkupAttribute, e));
                return new ClassifiedTextElement(
                    new ClassifiedTextRun(PredefinedClassificationTypeNames.WhiteSpace, "→ "),
                    new ClassifiedTextRun(PredefinedClassificationTypeNames.MarkupAttribute, e)
                );
            }));
            var content = new ContainerElement(ContainerElementStyle.Stacked, elements);

            return new QuickInfoItem(trackingSpan, content);
#endif
        }

        private QuickInfoItem? GetQuickInfoItem(IAsyncQuickInfoSession session)
        {
            ITextSnapshot snapshot = _famlModuleBuffer.TextBuffer.CurrentSnapshot;
            SnapshotPoint? triggerPoint = session.GetTriggerPoint(snapshot);
            if (triggerPoint == null)
                return null;
            int offset = triggerPoint.Value.Position;

            Api.QuickInfo.QuickInfo quickInfo = _famlModuleBuffer.FamlModule?.GetQuickInfo(triggerPoint.Value.Position);
            if (quickInfo == null)
                return null;

            ContainerElement containerElement;
            if (quickInfo is FunctionInvocationQuickInfo functionInvocationQuickInfo) {
                containerElement = new ContainerElement(
                    ContainerElementStyle.Stacked,
                    GetClassifiedTextElementForName(functionInvocationQuickInfo.FunctionName));
            }
            else return null;

            ITrackingSpan trackingSpan = snapshot.CreateTrackingSpan(quickInfo.Span.Start,
                quickInfo.Span.Length,
                SpanTrackingMode.EdgeInclusive);
            return new QuickInfoItem(trackingSpan, containerElement);
        }

        private ClassifiedTextElement GetClassifiedTextElementForName(QualifiableName qualifiableName) {
            if (qualifiableName.IsQualified())
                return new ClassifiedTextElement(
                    new ClassifiedTextRun(PredefinedClassificationTypeNames.Type,
                        qualifiableName.GetQualifier().ToString() + "."),
                    new ClassifiedTextRun(PredefinedClassificationTypeNames.Identifier,
                        qualifiableName.GetLastComponent().ToString())
                );
            else
                return new ClassifiedTextElement(
                    new ClassifiedTextRun(PredefinedClassificationTypeNames.Identifier,
                        qualifiableName.ToString()));
        }

#if false
        static async Task<Tuple<object, IEnumerable<string>>> RenderResourceValue(AndroidLayoutCompletionEngine engine, string resourceUrl) {
            var resourceValue = await engine.GetResourceValue(resourceUrl);
            if (resourceValue == null)
                return null;

            var attributeValue = resourceValue.Value;
            object renderedValue = null;
            List<string> path = new List<string>();
            if (resourceValue.ResolveChain != null) {
                path.Add(resourceUrl);
                path.AddRange(resourceValue.ResolveChain.Select(r => r.Label));
            }
            path.Add(attributeValue);

            if (AndroidResourceUtils.IsColorResource(resourceValue.Type, attributeValue)) {
                Xwt.Drawing.Color finalColor;
                ImageUtil.TryParseColor(attributeValue, out finalColor);
                return Tuple.Create(renderedValue = finalColor, (IEnumerable<string>)path);
            }

            if (AndroidResourceUtils.IsDrawableResource(resourceValue.Type, attributeValue)) {
                // Remove the reference to file paths from the list
                path.RemoveAll(File.Exists);
                var imgFilePath = attributeValue;
                if (imgFilePath.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                    renderedValue = engine.RenderXmlDrawable(resourceUrl).ImageViewer;
                else {
                    renderedValue = GetDrawableImage(resourceValue);
                }
                return Tuple.Create(renderedValue, (IEnumerable<string>)path);
            }

            return null;
        }

        static Xwt.ImageView GetDrawableImage(ResourceValue resValue) {
            int maxSize = 256;
            var image = Xwt.Drawing.Image.FromFile(resValue.Value);
            if (image.Width > maxSize || image.Height > maxSize)
                image = image.WithBoxSize(maxSize, maxSize);
            return new Xwt.ImageView(image);
        }
#endif
    }
}
