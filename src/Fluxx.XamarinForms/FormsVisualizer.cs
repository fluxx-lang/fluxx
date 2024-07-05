using System;
using System.Threading.Tasks;
using Faml.Api;
using Faml.App;
using Faml.Lang;

namespace Faml.XamarinForms {
    public class FormsVisualizer : IVisualizer {
        public Task<ExampleResult[]> VisualizeExample(QualifiableName moduleName, int exampleIndex) {
            throw new NotImplementedException();

#if LATER
            Device.BeginInvokeOnMainThread(async () => {
                ExampleResult[] exampleResults = appMqttClient.EvaluateExample(requestId, moduleName, exampleIndex);
                await VisualizeExampleResults(exampleResults);
            });
#endif
        }

#if LATER
        private async Task VisualizeExampleResults(ExampleResult[] exampleResults) {
            var examplePage = new ContentPage();

            var stackLayout = new StackLayout() {
                Orientation = StackOrientation.Vertical
            };

            foreach (ExampleResult exampleResult in exampleResults) {
                object resultContent = exampleResult.Content;

                if (resultContent is View view)
                    stackLayout.Children.Add(view);
                else {
                    stackLayout.Children.Add( new Label() {
                        Text = $"Unsupported type: {resultContent.GetType().FullName}"
                    });
                }

                string labelText = exampleResult.Label;
                if (!string.IsNullOrEmpty(labelText)) {
                    stackLayout.Children.Add( new Label() {
                        Text = labelText,
                        Margin = new Thickness(0, 10, 0, 0)
                    });
                }
            }

            examplePage.Content = stackLayout;

            await FamlXamarinFormsApplication.Instance.NavigateTo(examplePage);
        }
#endif
    }
}
