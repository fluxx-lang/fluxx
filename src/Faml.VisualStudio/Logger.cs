using System;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace Faml.VisualStudio {
    public class Logger {
        private static IVsOutputWindowPane? _pane;

        private static void Init() {
            ThreadHelper.ThrowIfNotOnUIThread();
            Guid famlOutputPaneGuid = new Guid("6B2640B1-5F23-4338-B15B-A5492C2A8360");

            // Create a new pane
            IVsOutputWindow outputWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
            outputWindow.CreatePane(
                ref famlOutputPaneGuid,
                "FAML",
                Convert.ToInt32(true),
                Convert.ToInt32(true));

            // Retrieve the new pane
            outputWindow.GetPane(ref famlOutputPaneGuid, out _pane);

            _pane.OutputString("This is the Created Pane \n");
        }

        public static void LogError(string message, Exception exception) {
            Log($"{message}: {exception.Message}");
        }

        public static async Task LogErrorAsync(string message, Exception exception) {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            LogError(message, exception);
        }

        private static void Log(string text) {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (_pane == null)
                Init();

            _pane.OutputString(text + "\n");
        }

        private static async Task LogAsync(string text) {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            Log(text);
        }
    }
}
