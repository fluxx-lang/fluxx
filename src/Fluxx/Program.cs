using System;
using Fluxx.Api;
using Fluxx.Lang;
using TypeTooling.Visualize;

namespace Fluxx
{
    public class Program
    {
        private readonly FamlProject rootProject;
        private DataEventHandler? dataEventHandler;
        private readonly string developmentMachine;

        public Program(FamlProject rootProject)
        {
            this.rootProject = rootProject;
        }

        public FamlProject RootProject => this.rootProject;

        public ExampleResult[] EvaluateExample(QualifiableName moduleName, int exampleIndex)
        {
            try
            {
                return this.rootProject.EvaluateExample(moduleName, exampleIndex);
            }
            catch (Exception e)
            {
                var exampleResult = new ExampleResult()
                {
                    Content = new VisualizableError(e.Message)
                };

                return new[] { exampleResult };
            }
        }

        public DataEventHandler? DataEventHandler
        {
            get => this.dataEventHandler;
            set => this.dataEventHandler = value;
        }
    }
}
