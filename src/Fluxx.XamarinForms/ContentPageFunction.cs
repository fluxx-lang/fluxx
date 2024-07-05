using System.Linq;
using Xamarin.Forms;
using System;
using Faml.Api;
using Faml.CodeAnalysis;
using Faml.Interpreter;
using ReactiveData;

namespace Faml.XamarinForms {
    public static class ContentPageFunction {
        public static IReactive<ContentPage> Invoke(IReactive<Program> program, string functionName, Args args) {
            return new ReactiveExpression<ContentPage>(() => DoInvoke(program, new QualifiableName(functionName), args));
        }

        private static ContentPage DoInvoke(IReactive<Program> program, QualifiableName functionName, Args args) {
            FamlProject project = program.Value.RootProject;

            if (project.AnyErrors) {
                Diagnostic firstProblem = project.GetAllDiagnostics().First();

                return new ContentPage() {
                    Content = new Label {
                        Text = firstProblem.Message,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                    }
                };
            }
            else {
                try {
                    Eval functionInvocationEval = project.CreateFunctionInvocationEval(functionName, args);

                    object page = ((FunctionInvocationObjectEval) functionInvocationEval).Eval();
                    return (ContentPage) page;
                }
                catch(Exception e) {
                    return new ContentPage() {
                        Content = new Label {
                            Text = e.ToString(),
                            VerticalOptions = LayoutOptions.CenterAndExpand,
                            HorizontalOptions = LayoutOptions.CenterAndExpand,
                        }
                    };
                }
            }
        }
    }
}
