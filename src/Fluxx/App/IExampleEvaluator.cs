using System.Threading.Tasks;
using Fluxx.Api;
using Fluxx.Lang;

namespace Fluxx.App
{
    public interface IExampleEvaluator
    {
        Task<ExampleResult[]> EvaluateExample(QualifiableName moduleName, int exampleIndex);
    }
}
