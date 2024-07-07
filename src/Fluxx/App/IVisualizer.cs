using System.Threading.Tasks;
using Fluxx.Api;
using Fluxx.Lang;

namespace Fluxx.App
{
    public interface IVisualizer
    {
        Task<ExampleResult[]> VisualizeExample(QualifiableName moduleName, int exampleIndex);
    }
}
