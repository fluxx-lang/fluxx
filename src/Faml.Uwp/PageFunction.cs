using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Faml.Api;
using Faml.CodeAnalysis;
using Faml.Interpreter;
using ReactiveData;

namespace Faml.Uwp {
    public static class PageFunction  {
        public static IReactive<Page> Invoke(IReactive<Program> program, string functionName, Args args) {
            return new ReactiveExpression<Page>(() => DoInvoke(program, new QualifiableName(functionName), args));
        }

        private static Page DoInvoke(IReactive<Program> program, QualifiableName functionName, Args args) {
            Page page = null;

            Task.Run(async () => {
                page = await DoInvokeAsync(program, functionName, args);
            }).Wait();

            return page;
        }

        private static async Task<Page> DoInvokeAsync(IReactive<Program> program, QualifiableName functionName, Args args) {
            try {
                FamlProject rootProject = program.Value.RootProject;

                if (rootProject.AnyErrors) {
                    Diagnostic firstProblem = rootProject.GetAllDiagnostics().First();

                    return await CreateOnUiThread(() =>
                        new Page() {
                            Content = new TextBlock {
                                Text = firstProblem.Message,
                            }
                        }).ConfigureAwait(false);
                }

                try {
                    Eval functionInvocationEval = rootProject.CreateFunctionInvocationEval(functionName, args);

                    return await CreateOnUiThread(() =>
                        (Page) ((FunctionInvocationObjectEval) functionInvocationEval).Eval()
                    ).ConfigureAwait(false);
                }
                catch (Exception e) {
                    return await CreateOnUiThread(() =>
                        new Page() {
                            Content = new TextBlock {
                                Text = e.ToString()
                            }
                        }).ConfigureAwait(false);
                }
            }
            catch (Exception e) {
                Debug.WriteLine($"Encountered error trying to recompute value: {e.Message}");
                return null;
            }
        }

        private static async Task<T> CreateOnUiThread<T>(Func<T> createFunction) {
            T value = default(T);
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () => {
                    value = createFunction();
                });

            return value;
        }
    }
}
