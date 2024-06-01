using Faml.Api;
using Faml.Lang;
using ReactiveData;

namespace Faml.DevEnv {
    public abstract class FamlExecutionContext {
        public abstract IReactive<ExampleResult[]> VisualizeExample(QualifiableName moduleName, int exampleIndex);
    }
}
