using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;

namespace Faml.VisualStudio.Taggers {
    internal static class FileAndContentTypeDefinitions {
        [Export]
        [Name("faml")]
        [BaseDefinition("code")]
        internal static ContentTypeDefinition FamlContentTypeDefinition;

        [Export]
        [FileExtension(".faml")]
        [ContentType(FamlPackage.FamlContentType)]
        internal static FileExtensionToContentTypeDefinition FamlFileExtensionDefinition;
    }
}
