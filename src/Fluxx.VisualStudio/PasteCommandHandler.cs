using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.IO;
using System.Windows;
using Fluxx.DevEnv;

// This code is based on https://github.com/madskristensen/PrettyPaste/blob/master/PrettyPaste/PasteCommandHandler.cs
namespace Fluxx.VisualStudio {
    internal class PasteCommandHandler : IOleCommandTarget {
        private readonly Guid _standardCommandSetGuid = VSConstants.GUID_VSStandardCommandSet97; // The VSConstants.VSStd97CmdID enumeration
        private readonly uint _pasteCommandId = (uint) VSConstants.VSStd97CmdID.Paste; // The paste command in the above enumeration

        private ITextView _textView;
        private readonly IOleCommandTarget _nextCommandTarget;
        private readonly DTE2 _dte;

        public PasteCommandHandler(IVsTextView adapter, ITextView textView, DTE2 dte) {
            _textView = textView;
            _dte = dte;
            adapter.AddCommandFilter(this, out _nextCommandTarget);
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut) {
            if (pguidCmdGroup == _standardCommandSetGuid && nCmdID == _pasteCommandId) {
                if (HandlePaste())
                   return VSConstants.S_OK;
            }

            return _nextCommandTarget.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }

        private bool HandlePaste() {
            if (!Clipboard.ContainsText(TextDataFormat.Text))
                return false;

            string xaml = Clipboard.GetText(TextDataFormat.Text);

            string lux;
            try {
                using (TextReader reader = new StringReader(xaml)) {
                    lux = new XamlToFaml().Convert(reader, 0);
                }
            }
            catch (Exception e) {
                return false;
            }

            // TODO: Check that conversion is valid

            TextDocument doc = (TextDocument) _dte.ActiveDocument.Object("TextDocument");
            EditPoint start = doc.Selection.TopPoint.CreateEditPoint();

            // First insert plain text
            _dte.UndoContext.Open("Paste");
            doc.Selection.Insert(xaml);
            _dte.UndoContext.Close();

            // Then replace with clean text, so undo restores the default behavior
            ReplaceText(lux, doc, start, xaml);

            return true;
        }

        private void ReplaceText(string updatedText, TextDocument doc, EditPoint start, string originalText) {
            _dte.UndoContext.Open("XAML to FAML");

            // Insert
            start.ReplaceText(originalText.Replace("\n", string.Empty).Length, updatedText, 0);
            //start.Insert(clean);

            /*
            // Format
            doc.Selection.MoveToPoint(start, true);
            formatSelection();
            _textView.Selection.Clear();
            */

            _dte.UndoContext.Close();
        }

        private void FormatSelection() {
            Command command = _dte.Commands.Item("Edit.FormatSelection");

            if (command.IsAvailable) {
                _dte.ExecuteCommand("Edit.FormatSelection");
            }
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText) {
            if (pguidCmdGroup == _standardCommandSetGuid) {
                for (int i = 0; i < cCmds; i++) {
                    if (prgCmds[i].cmdID == _pasteCommandId) {
                        prgCmds[i].cmdf = (uint)(OLECMDF.OLECMDF_ENABLED | OLECMDF.OLECMDF_SUPPORTED);
                        return VSConstants.S_OK;
                    }
                }
            }

            return _nextCommandTarget.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }
    }
}
