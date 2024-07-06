using System;

namespace TypeTooling.Visualize
{
    [Serializable]
    public class VisualizableError(string message)
    {
        // Auto properties
        public string Message { get; } = message;
    }
}
