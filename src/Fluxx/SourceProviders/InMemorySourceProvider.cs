using System.Collections.Generic;
using System.Collections.Immutable;

namespace Faml.SourceProviders
{
    public class InMemorySourceProvider : SourceProvider
    {
        private readonly Dictionary<string, object> resources = new Dictionary<string, object>();

        public void AddTextResource(string path, string contents)
        {
            this.resources.Add(path, contents);
        }

        public void AddBinaryResource(string path, byte[] contents)
        {
            this.resources.Add(path, contents);
        }

        public override string? GetTextResource(string path) =>
            this.resources.TryGetValue(path, out object contents) ? (string)contents : null;

        public override ImmutableArray<byte>? GetBinaryResource(string path) =>
            this.resources.TryGetValue(path, out object contents) ? (ImmutableArray<byte>?)contents : null;

        public override IEnumerable<string> GetResources()
        {
            return this.resources.Keys;
        }

        public override string RootPath => "in-memory";
    }
}
