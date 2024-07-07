using System.Linq;
using System;
using System.Windows;
using System.Windows.Controls;
using Fluxx.Api;
using Fluxx.CodeAnalysis;
using ReactiveData;

namespace Fluxx.Wpf {
    public static class UIElementFunction {
        public static IReactive<UIElement> Invoke(IReactive<Program> program, string functionName, Args args) {
            return new ReactiveExpression<UIElement>(() => DoInvoke(program, new QualifiableName(functionName), args));
        }

        private static UIElement DoInvoke(IReactive<Program> program, QualifiableName functionName, Args args) {
            FamlProject rootProject = program.Value.RootProject;

            if (rootProject.AnyErrors) {
                Diagnostic firstProblem = rootProject.GetAllDiagnostics().First();

                return new TextBlock {
                    Text = firstProblem.Message
                };
            }

            try {
                Delegate functionInvocationDelegate = rootProject.CreateFunctionInvocationDelegate(functionName, args);

                object uiElement = null;
                Application.Current.Dispatcher.Invoke(() => {
                    uiElement = functionInvocationDelegate.DynamicInvoke();
                });

                return (UIElement) uiElement;
            }
            catch (Exception e) {
                return new TextBlock {
                    Text = e.ToString()
                };
            }
        }
    }
}
