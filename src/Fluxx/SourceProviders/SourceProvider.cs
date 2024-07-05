using System.Collections.Generic;
using System.Collections.Immutable;

namespace Faml.SourceProviders {
    /// <summary>
    /// A SourceProvider provides FAML source code and resource files, in a hierarchical namespace.  It's an
    /// abstraction layer, allowing those files to be stored on disk in the a normal directory structure (via
    /// FileSourceProvider) or hosted in some other way (e.g. embedded in a C# assembly as resources or in a
    /// zip file).
    /// </summary>
    public abstract class SourceProvider {
        public abstract string? GetTextResource(string path);

        public virtual ImmutableArray<byte>? GetBinaryResource(string path) {
            return null;
        }

        public abstract IEnumerable<string> GetResources();

        public abstract string RootPath { get; }
    }
}
