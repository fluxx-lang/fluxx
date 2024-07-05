using System.Linq;
using Faml.Api;
using Faml.Lang;
using ReactiveData;

namespace Faml.DevEnv
{
    public class LocalExecutionContext : FamlExecutionContext {
        private readonly FamlProject _project;

        public LocalExecutionContext(FamlProject project) {
            _project = project;
        }

        public override IReactive<ExampleResult[]> VisualizeExample(QualifiableName moduleName, int exampleIndex) {
            ExampleResult[] exampleResults = _project.EvaluateExample(moduleName, exampleIndex).ToArray();
            return new ReactiveConstant<ExampleResult[]>(exampleResults);
        }
    }
}
