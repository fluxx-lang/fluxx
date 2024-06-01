using System.Collections.Generic;
using System.Collections.Immutable;

namespace Faml.SourceProviders {
    public class InMemorySourceProvider : SourceProvider {
        private readonly Dictionary<string, object> _resources = new Dictionary<string, object>();


        public void AddTextResource(string path, string contents) {
            _resources.Add(path, contents);
        }

        public void AddBinaryResource(string path, byte[] contents) {
            _resources.Add(path, contents);
        }

        public override string? GetTextResource(string path) =>
            _resources.TryGetValue(path, out object contents) ? (string) contents : null;

        public override ImmutableArray<byte>? GetBinaryResource(string path) =>
            _resources.TryGetValue(path, out object contents) ? (ImmutableArray<byte>?)contents : null;

        public override IEnumerable<string> GetResources() {
            return _resources.Keys;
        }

        public override string RootPath => "in-memory";
    }
}
