using System.Collections.Generic;
using System.Globalization;
using Faml.SourceProviders;

namespace Faml {
    /// <summary>
    /// A Workspace manages a collection of projects, which may reference each other and are normally built together.
    /// It corresponds to a solution in Visual Studio.
    /// </summary>
    public class FamlWorkspace {
        private readonly List<FamlProject> _projects = new List<FamlProject>();

        public FamlProject CreateProject(SourceProvider sourceProvider) {
            var project = new FamlProject(this, sourceProvider);
            _projects.Add(project);

            return project;
        }

        public FamlProject CreateProject() {
            return CreateProject(new NullSourceProvider());
        }

        public FamlProject CreateProject(string projectDirectory) {
            return CreateProject(new FileSourceProvider(projectDirectory));
        }

        public CultureInfo UICulture { get; set; } = CultureInfo.CurrentUICulture;
    }
}
