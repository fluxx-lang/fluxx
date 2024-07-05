using System;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Faml.Api;
using Faml.App;
using Faml.Lang;
using ReactiveData;

namespace Faml.Uwp {
    public class UwpExampleEvaluator : IExampleEvaluator {
        private readonly IReactive<Program> _program;
        private readonly Page _mainPage;


        public UwpExampleEvaluator(IReactive<Program> program, Page mainPage) {
            _program = program;
            _mainPage = mainPage;
        }

        public async Task<ExampleResult[]> EvaluateExample(QualifiableName moduleName, int exampleIndex) {
            ExampleResult[] exampleResults = null;
            // Since examples object contain UI objects, switch to the UI thread to create and render them
            await _mainPage.Dispatcher.RunAsync(CoreDispatcherPriority.High,
                () => {
                    exampleResults = _program.Value.EvaluateExample(moduleName, exampleIndex);
                });
            return exampleResults;
        }
    }
}
