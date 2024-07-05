using System;

namespace TypeTooling.Visualize
{
    [Serializable]
    public class VisualizableError
    {
        // Auto properties
        public string Message { get; }

        public VisualizableError(string message)
        {
            Message = message;
        }
    }
}
