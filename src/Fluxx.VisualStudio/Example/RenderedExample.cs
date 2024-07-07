using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Fluxx.Lang;
using ReactiveData;
using TypeTooling.Visualize;

namespace Fluxx.VisualStudio.Example
{
    public delegate void PresentationAvailableEventHandler();

    public class RenderedExample {
        private readonly ExampleManager _exampleManager;
        private readonly int _exampleIndex;
        private UIElement _uiElement;
        private double _heightNeeded;
        private readonly PresentationAvailableEventHandler _presentationAvailableEventHandler;

        internal RenderedExample(ExampleManager exampleManager, int exampleIndex, IReactive<ExampleResult[]> exampleResults,
            SynchronizationContext uiSynchronizationContext, PresentationAvailableEventHandler presentationAvailableEventHandler) {

            _exampleManager = exampleManager;
            _exampleIndex = exampleIndex;
            _presentationAvailableEventHandler = presentationAvailableEventHandler;

            if (exampleResults.Value != null)
                uiSynchronizationContext.Post(CreatePresentation, exampleResults.Value);

            exampleResults.Changed += (() => {
                uiSynchronizationContext.Post(CreatePresentation, exampleResults.Value);
            });
        }

        public int ExampleIndex => _exampleIndex;

        public bool IsPresentationAvailable => _uiElement != null;

        public UIElement UiElement => _uiElement;

        public double HeightNeeded => _heightNeeded;

        private void CreatePresentation(object exampleResultsObject) {
            var exampleResults = (ExampleResult[]) exampleResultsObject;

            // For now, just look at the first result
            object firstContent = exampleResults[0].Content;

            if (firstContent is MimeData mimeData)
                firstContent = RenderMimeBitmapImage(mimeData);

            if (firstContent is BitmapImage bitmapImageValue) {
                var stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Horizontal;

                int maxHeightNeeded = 0;
                foreach (ExampleResult exampleResult in exampleResults) {
                    object content = exampleResult.Content;
                    if (content is MimeData contentMimeData)
                        content = RenderMimeBitmapImage(contentMimeData);

                    var image = new Image {
                        Source = (BitmapImage) content,
                    };
                    
                    var imageExample = new ImageExample(image, exampleResult.Label);

                    stackPanel.Children.Add(imageExample.RootElement);
                    imageExample.RootElement.Margin = new Thickness(0, 0, 10, 0);

                    // TODO: Fix this up (somehow) to use ActualHeight to get exact height of text & handle margins better
                    int heightNeeded = bitmapImageValue.PixelHeight + 10;
                    if (exampleResult.Label != null)
                        heightNeeded += 25;

                    maxHeightNeeded = Math.Max(maxHeightNeeded, heightNeeded);
                }

                _uiElement = stackPanel;
                _heightNeeded = maxHeightNeeded;
            }
            else if (firstContent is VisualizableError visualizableError) {
                var run = new Run(visualizableError.Message);
                var textBlock = new TextBlock(run) {
                    Foreground = new SolidColorBrush(Colors.Red)
                };

                // TODO: Test that height here is set, since it's not flowed yet
                _heightNeeded = textBlock.Height;
                _uiElement = textBlock;
            }
            else {
                string stringValue = firstContent.ToString();

                var run = new Run("= " + stringValue);
                var textBlock = new TextBlock(run) {
                    Foreground = new SolidColorBrush(Colors.Wheat)
                };

                // TODO: Test that height here is set, since it's not flowed yet
                _heightNeeded = textBlock.Height;
                _uiElement = textBlock;
            }

            _exampleManager.UpdatePreviousRenderedExamples(this);

            _presentationAvailableEventHandler();
        }

        public static BitmapImage? RenderMimeBitmapImage(MimeData mimeData) {
            // We only support PNG data currently
            string mimeType = mimeData.MimeType;
            if (mimeType != MimeData.MimeTypeImagePng)
                return null;

            using var memoryStream = new MemoryStream(mimeData.Data);

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.EndInit();
            return bitmapImage;
        }

#if false
        [CanBeNull] public static WriteableBitmap RenderSvg(Svg svg) {
            int width = (int)svg.width;
            int height = (int)svg.height;

            // If the width/height are invalid--or too big--then don't render anything
            if (width <= 0 || width > 1000 || height <= 0 || height > 1000)
                return null;

            var writeableBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, BitmapPalettes.Halftone256Transparent);

            writeableBitmap.Lock();
            using (var surface = SKSurface.Create(
                width: width,
                height: height,
                colorType: SKColorType.Bgra8888,
                alphaType: SKAlphaType.Premul,
                pixels: writeableBitmap.BackBuffer,
                rowBytes: width * 4)) {

                SKCanvas canvas = surface.Canvas;

                canvas.Clear(new SKColor(0, 0, 0));

                var svgRenderer = new SvgRenderer(canvas);
                svgRenderer.DrawSvg(svg);

                //canvas.DrawText("SkiaSharp on Wpf!", 50, 59, new SKPaint() { Color = new SKColor(0, 128, 0), TextSize = 100 });
            }

            writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));

            writeableBitmap.Unlock();

            return writeableBitmap;
        }
#endif
    }
}
