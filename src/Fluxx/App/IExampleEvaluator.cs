using System.Threading.Tasks;
using Faml.Api;
using Faml.Lang;

namespace Faml.App
{
    public interface IExampleEvaluator
    {
        Task<ExampleResult[]> EvaluateExample(QualifiableName moduleName, int exampleIndex);
    }
}
