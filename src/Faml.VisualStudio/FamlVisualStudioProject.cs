using System;
using System.Collections.Generic;
using System.IO;
using Faml.Api;
using Faml.VisualStudio.VsUtil;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Constants = EnvDTE.Constants;

namespace Faml.VisualStudio {
    public class FamlVisualStudioProject {
        private readonly FamlVisualStudioWorkspace _famlVisualStudioWorkspace;
        private readonly FamlProject _famlProject;
        private readonly RoslynDotNetRawTypeProvider _roslynDotNetRawTypeProvider;
        public string FamlProjectRootDirectory { get; }
        public Guid ProjectGuidInSolution { get; }

#if false
        public static FamlVisualStudioProject GetOrCreateFromHierarchy(IVsHierarchy hierarchy) {
            ThreadHelper.ThrowIfNotOnUIThread();

            // VSITEMID_ROOT gets the current project. 
            // Alternativly, you might have another item id.
            var itemid = VSConstants.VSITEMID_ROOT;

            hierarchy.GetProperty(itemid, (int)__VSHPROPID.VSHPROPID_ExtObject, out object objProj);

            Project dteProject = objProj as EnvDTE.Project;

            dteProject.Properties.Get

            return textBuffer.Properties.GetOrCreateSingletonProperty(() => Create(textBuffer));
        }
#endif

        public FamlVisualStudioProject(FamlVisualStudioWorkspace famlVisualStudioWorkspace, IVsHierarchy hierarchy, Guid projectGuidInSolution) {
            ThreadHelper.ThrowIfNotOnUIThread();

            _famlVisualStudioWorkspace = famlVisualStudioWorkspace;
            ProjectGuidInSolution = projectGuidInSolution;

            if (! (hierarchy is IVsProject project))
                throw new InvalidOperationException("IVsHierarchy couldn't be cast to IVsProject; enhance code to support a more sophisticated conversion");

            string projectFilePath = project.GetFilePath();
            FamlProjectRootDirectory = Path.GetDirectoryName(projectFilePath);

            _famlProject = FamlVisualStudioWorkspace.FamlWorkspace.CreateProject(FamlProjectRootDirectory);

            _roslynDotNetRawTypeProvider = new RoslynDotNetRawTypeProvider(project);
            _famlProject.DotNetProjectInfo.RawTypeProvider = _roslynDotNetRawTypeProvider;

            _roslynDotNetRawTypeProvider.CompilationChanged += (o, args) => {
                _famlProject.ExternalDependenciesChanged();
            };
        }

#if false
        [CanBeNull] public Project GetRoslynProject() {
            VisualStudioWorkspace roslynWorkspace = FamlPackagePrevious.Instance.RoslynWorkspace;
            Solution currentSolution = roslynWorkspace.CurrentSolution;
            if (currentSolution == null)
                return null;

            var luxProjectDirectoryUri = new Uri(_projectRootDirectory + Path.PathSeparator);

            foreach (Project roslynProject in currentSolution.Projects) {
                string projectPath = roslynProject.FilePath;
                if (projectPath == null)
                    continue;

                var roslynProjectDirectoryUri = new Uri(projectPath + Path.PathSeparator);

                if (roslynProjectDirectoryUri.IsBaseOf(luxProjectDirectoryUri))
                    return roslynProject;
            }

            return null;
        }
#endif

        private static void AddSolutionFolderProjects(EnvDTE.Project solutionFolder, List<EnvDTE.Project> projects) {
            ThreadHelper.ThrowIfNotOnUIThread();

            for (var i = 1; i <= solutionFolder.ProjectItems.Count; i++) {
                EnvDTE.Project subProject = solutionFolder.ProjectItems.Item(i).SubProject;
                if (subProject == null)
                    continue;

                // If this is another solution folder, do a recursive call, otherwise add
                if (subProject.Kind == Constants.vsProjectKindSolutionItems)
                    AddSolutionFolderProjects(subProject, projects);
                else projects.Add(subProject);
            }
        }

        public FamlVisualStudioWorkspace FamlVisualStudioWorkspace => _famlVisualStudioWorkspace;

        public FamlProject FamlProject => _famlProject;

        public string GetRelativePath(string absolutePath) {
            if (!absolutePath.StartsWith(FamlProjectRootDirectory))
                throw new Exception(
                    $"Path {absolutePath} is unexpectedly not under project root {FamlProjectRootDirectory}");

            return absolutePath.Substring(FamlProjectRootDirectory.Length + 1);
        }

        public string? GetModuleFullPath(QualifiableName moduleName) {
            string moduleRelativePath = moduleName.ToRelativePath();
            return Path.Combine(FamlProjectRootDirectory, moduleRelativePath);
        }
    }
}
