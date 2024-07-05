using System;
using Faml.Api;
using Faml.Lang;
using TypeTooling.Visualize;

namespace Faml {
    public class Program {
        private readonly FamlProject _rootProject;
        private DataEventHandler? _dataEventHandler;
        private readonly string _developmentMachine;


        public Program(FamlProject rootProject) {
            _rootProject = rootProject;
        }

        public FamlProject RootProject => _rootProject;

        public ExampleResult[] EvaluateExample(QualifiableName moduleName, int exampleIndex) {
            try {
                return _rootProject.EvaluateExample(moduleName, exampleIndex);
            }
            catch (Exception e) {
                var exampleResult = new ExampleResult() {
                    Content = new VisualizableError(e.Message)
                };

                return new[] { exampleResult };
            }
        }

        public DataEventHandler? DataEventHandler {
            get => _dataEventHandler;
            set => _dataEventHandler = value;
        }
    }
}
