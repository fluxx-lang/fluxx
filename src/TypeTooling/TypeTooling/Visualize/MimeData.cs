using System;

namespace TypeTooling.Visualize
{
    /// <summary>
    /// The MimeData type can be returned from Visualizer.Visualize, where the visualization is serialized and
    /// there's a standard MIME type to describe it (e.g. image/png).
    /// </summary>
    [Serializable]
    public class MimeData(string mimeType, byte[] data)
    {
        // Define some commonly used MIME types, though any valid MIME type is allowed for this class
        public static readonly string MimeTypeImagePng = "image/png";

        // Auto properties
        public string MimeType { get; } = mimeType;

        public byte[] Data { get; } = data;
    }
}
