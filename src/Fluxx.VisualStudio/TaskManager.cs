using System;
using Fluxx.Api;
using Microsoft.CodeAnalysisP;
using Microsoft.CodeAnalysisP.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Diagnostic = Fluxx.CodeAnalysis.Diagnostic;
using DiagnosticSeverity = Fluxx.Api.DiagnosticSeverity;
using TextSpan = Microsoft.CodeAnalysisP.Text.TextSpan;

namespace Fluxx.VisualStudio {
    public sealed class TaskManager {
        private readonly ErrorListProvider _errorListProvider;

        public TaskManager(IServiceProvider serviceProvider) {
            _errorListProvider = new ErrorListProvider(serviceProvider);//this implementing IServiceProvider
            //provider.ProviderName = { name scoping package};
            //provider.ProviderGuid = { guid scoping package};
        }

        public void AddDiagnostic(string? fileName, Diagnostic diagnostic) {
            TaskErrorCategory errorCategory;
            switch (diagnostic.Severity) {
                case DiagnosticSeverity.Warning:
                    errorCategory = TaskErrorCategory.Warning;
                    break;
                case DiagnosticSeverity.Info:
                    errorCategory = TaskErrorCategory.Message;
                    break;
                default:
                    errorCategory = TaskErrorCategory.Error;
                    break;
            }

            // Newlines show up in the info popup, in the editor, but cause the message to be truncated in the task list,
            // so turn any newlines into spaces there
            // TODO: Fix up formatting here some more
            string message = diagnostic.Message;
            message = message.Replace('\n', ' ');

            var errorTask = new ErrorTask {
                Category = TaskCategory.User,
                ErrorCategory = errorCategory,
                Text = message,
            };

            LinePosition? startLinePosition = diagnostic.StartLinePosition;
            if (startLinePosition != null) {
                errorTask.Line = startLinePosition.Value.Line;
                errorTask.Column = startLinePosition.Value.Character;
            }

            if (fileName != null) {
                errorTask.Document = fileName;
                errorTask.Navigate += NavigateToError;
            }

            _errorListProvider.Tasks.Add(errorTask);
        }

        void NavigateToError(object sender, EventArgs e) {
            if (!(sender is Task task))
                throw new ArgumentException("sender");

            //use the helper class to handle the navigation
            OpenDocumentAndNavigateTo(task.Document, task.Line, task.Column);
        }

        // This code was taken from https://vsxexperience.net/2010/03/23/writing-to-the-vs-errorlist/
        public static void OpenDocumentAndNavigateTo(string path, int line, int column) {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (!(Package.GetGlobalService(typeof(IVsUIShellOpenDocument)) is IVsUIShellOpenDocument openDoc))
                return;

            Guid logicalView = VSConstants.LOGVIEWID_Code;
            if (ErrorHandler.Failed(openDoc.OpenDocumentViaProject(
                    path,
                    ref logicalView,
                    out Microsoft.VisualStudio.OLE.Interop.IServiceProvider _,
                    out IVsUIHierarchy _,
                    out uint _,
                    out IVsWindowFrame frame)) || frame == null)
                return;

            frame.GetProperty((int)__VSFPROPID.VSFPROPID_DocData, out object docData);

            // Get the VsTextBuffer  
            var buffer = docData as VsTextBuffer;
            if (buffer == null) {
                if (docData is IVsTextBufferProvider bufferProvider) {
                    ErrorHandler.ThrowOnFailure(bufferProvider.GetTextBuffer(out IVsTextLines lines));
                    buffer = lines as VsTextBuffer;
                    if (buffer == null)
                        return;
                }
            }

            // Finally, perform the navigation.  
            if (!(Package.GetGlobalService(typeof(VsTextManagerClass)) is IVsTextManager mgr))
                return;
            mgr.NavigateToLineAndColumn(buffer, ref logicalView, line, column, line, column);
        }

        public void ClearTasks() {
            _errorListProvider.Tasks.Clear();
        }
    }
}
