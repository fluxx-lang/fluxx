using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;

namespace LuxVsix {
    internal static class FileAndContentTypeDefinitions {
        [Export]
        [Name("lux")]
        [BaseDefinition("code")]
        internal static ContentTypeDefinition LuxContentType = null;

        [Export]
        [FileExtension(".lux")]
        [ContentType("lux")]
        internal static FileExtensionToContentTypeDefinition LuxFileType = null;
    }
}