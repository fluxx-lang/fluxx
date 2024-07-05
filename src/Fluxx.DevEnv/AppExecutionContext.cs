using Faml.Api;
using Faml.Lang;
using ReactiveData;

namespace Faml.DevEnv
{
    public class AppExecutionContext : FamlExecutionContext {
        private readonly DevEnvToAppConnection _devEnvToAppConnection;

        public AppExecutionContext(DevEnvToAppConnection devEnvToAppConnection) {
            _devEnvToAppConnection = devEnvToAppConnection;
        }

        public override IReactive<ExampleResult[]> VisualizeExample(QualifiableName moduleName, int exampleIndex) {
            return _devEnvToAppConnection.VisualizeExample(moduleName, exampleIndex);
        }
    }
}
