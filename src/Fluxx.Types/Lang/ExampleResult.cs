using System;
using TypeTooling.CompanionType;

namespace Fluxx.Lang
{
    [Serializable]
    public sealed class ExampleResult
    {
        // Automatic properties
        public string Label { get; set; } = null;

        public object Content { get; set; }
    }

    public sealed class ExampleResultTypeTooling : IContentPropertyProvider
    {
        public string GetContentProperty() => nameof(ExampleResult.Content);
    }
}
