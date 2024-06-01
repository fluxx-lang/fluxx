using System;

namespace TypeTooling.Visualize {
    /// <summary>
    /// The MimeData type can be returned from Visualizer.Visualize, where the visualization is serialized and
    /// there's a standard MIME type to describe it (e.g. image/png).
    /// </summary>
    [Serializable]
    public class MimeData {
        // Define some commonly used MIME types, though any valid MIME type is allowed for this class
        public static readonly string MimeTypeImagePng = "image/png";

        // Auto properties
        public string MimeType { get; }
        public byte[] Data { get; }

        // TDOO: Consider changing implementation to use IntPtr if possible to avoid extra copying when serialize
        // See https://stackoverflow.com/questions/2206961/sharing-data-between-appdomains/11665093

        public MimeData(string mimeType, byte[] data) {
            MimeType = mimeType;
            Data = data;
        }
    }
}
