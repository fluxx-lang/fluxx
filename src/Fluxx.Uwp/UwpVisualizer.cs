using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Faml.Api;
using Faml.App;
using Faml.Lang;
using ReactiveData;
using TypeTooling.Visualize;

namespace Faml.Uwp {
    public class UwpVisualizer : IVisualizer {
        private readonly IReactive<Program> _program;
        private readonly Page _mainPage;

        public UwpVisualizer(IReactive<Program> program, Page mainPage) {
            _program = program;
            _mainPage = mainPage;
        }

        public async Task<ExampleResult[]> VisualizeExample(QualifiableName moduleName, int exampleIndex) {
            ExampleResult[] exampleResults = null;
            await _mainPage.Dispatcher.RunAsync(CoreDispatcherPriority.High, async () => {
                    exampleResults = _program.Value.EvaluateExample(moduleName, exampleIndex);
                    for (var i = 0; i < exampleResults.Length; i++)
                        exampleResults[i] = await RenderExampleResult(exampleResults[i]);
            });
            return exampleResults;
        }

        private async Task<ExampleResult> RenderExampleResult(ExampleResult exampleResult) {
            var uiElement = (UIElement) exampleResult.Content;

            var renderTargetBitmap = new RenderTargetBitmap();

            _mainPage.Content = uiElement;
            await renderTargetBitmap.RenderAsync(uiElement);

            IBuffer pixelBuffer = await renderTargetBitmap.GetPixelsAsync();

            using var memoryStream = new InMemoryRandomAccessStream();
            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, memoryStream);

            uint pixelWidth = (uint) renderTargetBitmap.PixelWidth;
            uint pixelHeight = (uint) renderTargetBitmap.PixelHeight;

            encoder.SetPixelData(
                BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Ignore,
                pixelWidth,
                pixelHeight,
                DisplayInformation.GetForCurrentView().LogicalDpi,
                DisplayInformation.GetForCurrentView().LogicalDpi,
                pixelBuffer.ToArray());

            await encoder.FlushAsync();
            memoryStream.Seek(0);

            byte[] bytes = new byte[memoryStream.Size];
            await memoryStream.AsStream().ReadAsync(bytes, 0, bytes.Length);

            return new ExampleResult {
                Label = exampleResult.Label,
                Content = new MimeData (MimeData.MimeTypeImagePng, bytes)
            };
        }
    }
}
