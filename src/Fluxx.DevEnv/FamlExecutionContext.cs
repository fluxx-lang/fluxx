using Fluxx.Api;
using Fluxx.Lang;
using ReactiveData;

namespace Fluxx.DevEnv {
    public abstract class FamlExecutionContext {
        public abstract IReactive<ExampleResult[]> VisualizeExample(QualifiableName moduleName, int exampleIndex);
    }
}
