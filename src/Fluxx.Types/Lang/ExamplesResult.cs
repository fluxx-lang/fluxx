using System.Collections.Generic;
using TypeTooling.CompanionType;

namespace Faml.Lang
{
    public sealed class ExamplesResult
    {
        // Auto properties
        public List<ExampleResult> Content { get; } = new List<ExampleResult>();
    }

    public sealed class ExamplesResultTypeTooling : IContentPropertyProvider
    {
        public string GetContentProperty() => nameof(ExamplesResult.Content);
    }
}
