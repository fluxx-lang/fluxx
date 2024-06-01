using System;
using System.Text;
using Faml.Api;
using Faml.CodeAnalysis;
using Faml.Lang;
using Faml.Syntax;
using Faml.VisualStudio.Example;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using ReactiveData;
using InvalidOperationException = System.InvalidOperationException;

namespace Faml.VisualStudio {
    public class FamlModuleBuffer {
        private readonly ITextBuffer _textBuffer;
        private readonly ITextDocument _textDocument;
        private readonly FamlVisualStudioProject _famlVisualStudioProject;
        private FamlModule? _famlModule;
        private readonly string _relativePath;
        /// <summary>
        /// All examples that we've rendered or are in the process of rendering, by example index
        /// </summary>
        private readonly ExampleManager _exampleManager;

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public static FamlModuleBuffer GetOrCreateFromTextBuffer(ITextBuffer textBuffer) {
            ThreadHelper.ThrowIfNotOnUIThread();
            return textBuffer.Properties.GetOrCreateSingletonProperty(() => Create(textBuffer));
        }

        private static FamlModuleBuffer Create(ITextBuffer textBuffer) {
            ThreadHelper.ThrowIfNotOnUIThread();

            ITextDocumentFactoryService textDocumentFactoryService = FamlPackage.Instance.TextDocumentFactoryService;

            if (!textDocumentFactoryService.TryGetTextDocument(textBuffer, out ITextDocument textDocument))
                throw new InvalidOperationException("Could not get ITextDocument from ITextBuffer");

            string filePath = textDocument.FilePath;

            IVsHierarchy hierarchy = GetHierarchyForDocument(filePath);
            if (hierarchy == null)
                throw new InvalidOperationException($"Document {filePath} not found in any project");

            FamlVisualStudioProject famlVisualStudioProject = FamlPackage.Instance.GetOrCreateFamlWorkspace().GetSingleProject(hierarchy);

#if false

            string projectRootDirectory = DevEnvUtil.GetProjectRootDirectoryForFile(filePath);
            if (projectRootDirectory == null)
                projectRootDirectory = Path.GetDirectoryName(filePath);

            AdHocFamlVisualStudioWorkspace famlVisualStudioWorkspace = AdHocVisualStudioWorkspaces.GetOrCreateAdHocWorkspace(projectRootDirectory, textDocumentFactoryService);
#endif

            return new FamlModuleBuffer(textBuffer, textDocument, famlVisualStudioProject);
        }

        private static IVsHierarchy GetHierarchyForDocument(string filePath) {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (!(Package.GetGlobalService(typeof(SVsSolution)) is IVsSolution2 solution))
                return null;

            Guid guid = Guid.Empty;
            solution.GetProjectEnum((uint)__VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION, ref guid, out IEnumHierarchies enumHierarchies);
            if (enumHierarchies == null)
                return null;

            var hierarchies = new IVsHierarchy[1];
            while (enumHierarchies.Next(1, hierarchies, out var fetched) == VSConstants.S_OK && fetched == 1) {
                IVsHierarchy hierarchy = hierarchies[0];
                if (hierarchy == null)
                    continue;

                if (ErrorHandler.Succeeded(hierarchy.ParseCanonicalName(filePath, out uint itemId)) &&
                    itemId != (uint) VSConstants.VSITEMID.Nil)
                    return hierarchy;
            }

            return null;
        }

        public FamlModuleBuffer(ITextBuffer textBuffer, ITextDocument textDocument, FamlVisualStudioProject famlVisualStudioProject) {
            _textBuffer = textBuffer;
            _textDocument = textDocument;

            _famlVisualStudioProject = famlVisualStudioProject;
            _exampleManager = new ExampleManager(this);

            _relativePath = _famlVisualStudioProject.GetRelativePath(FullPath);

            _textBuffer.Changed += textBuffer_Changed;

            OnBufferChanged();
        }

        public ITextBuffer TextBuffer => _textBuffer;

        public string FullPath => _textDocument.FilePath;

        public string RelativePath => _relativePath;

        public QualifiableName ModuleName => QualifiableName.ModuleNameFromRelativePath(_relativePath);

        public FamlProject? Project => _famlVisualStudioProject.FamlProject;

        public FamlModule? FamlModule => _famlModule;

        public FamlVisualStudioWorkspace FamlVisualStudioWorkspace => _famlVisualStudioProject.FamlVisualStudioWorkspace;

        public ExampleManager ExampleManager => _exampleManager;

        private void textBuffer_Changed(object sender, TextContentChangedEventArgs eventArgs) {
            try {
                OnBufferChanged();
            }
            catch (Exception e) {
                Logger.LogError("textBuffer_Changed failed", e);
            }
        }

        public void NotifyTagsChanged(SnapshotSpanEventArgs e) {
            if (TagsChanged != null)
                TagsChanged(this, e);
        }

        private void OnBufferChanged() {
            string newSource = _textBuffer.CurrentSnapshot.GetText();

            _famlModule = null;
            _exampleManager.Clear();

            FamlProject project = _famlVisualStudioProject.FamlProject;
            try {
                project.UpdateSource(_relativePath, newSource);
            }
            catch (Exception e) {
                System.Diagnostics.Debug.WriteLine($"Error when calling UpdateSource: {e}");
                return;
            }

            _famlModule = project.GetModuleIfExists(QualifiableName.ModuleNameFromRelativePath(_relativePath));

            var sourceSnapshotSpan = new SnapshotSpan(_textBuffer.CurrentSnapshot, 0, newSource.Length);
            var snapshotSpanEventArgs = new SnapshotSpanEventArgs(sourceSnapshotSpan);
            NotifyTagsChanged(snapshotSpanEventArgs);

            TaskManager taskManager = FamlPackage.Instance.TaskManager;
            taskManager.ClearTasks();
            foreach (Diagnostic diagnostic in project.GetAllDiagnostics()) {
                QualifiableName moduleName = diagnostic.ModuleName;

                string moduleFullPath = null;
                if (!moduleName.IsEmpty())
                    moduleFullPath = _famlVisualStudioProject.GetModuleFullPath(moduleName);

                taskManager.AddDiagnostic(moduleFullPath, diagnostic);
            }

            if (!project.AnyErrors && _famlModule != null) {
                UpdateDevice(newSource);
                //updateDeviceIOS(source);
            }
        }

        private void UpdateDevice(string source) {
            try {
                //await _projectInfo.appDevEnvMqttClient.startSubscription();
                if (FamlVisualStudioWorkspace.AppConnection != null)
                    FamlVisualStudioWorkspace.AppConnection.UpdateSource(_relativePath, source);
            }
            catch (Exception e) {
                Logger.LogError("UpdateDevice failed", e);
            }
        }

        // TODO: Can this be nullable?
        public IReactive<ExampleResult[]> VisualizeExample(int exampleIndex) {
            return FamlVisualStudioWorkspace.AppConnection?.VisualizeExample(ModuleName, exampleIndex);
        }
    }
}
