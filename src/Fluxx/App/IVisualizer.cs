using System.Threading.Tasks;
using Faml.Api;
using Faml.Lang;

namespace Faml.App
{
    public interface IVisualizer
    {
        Task<ExampleResult[]> VisualizeExample(QualifiableName moduleName, int exampleIndex);
    }
}
