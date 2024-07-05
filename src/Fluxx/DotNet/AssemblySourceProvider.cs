using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Faml.SourceProviders;

namespace Faml.DotNet {
    public class AssemblySourceProvider : SourceProvider {
        private readonly List<Assembly> _assemblies;

        public AssemblySourceProvider(Assembly assembly) {
            _assemblies = new List<Assembly> {assembly};
        }

        public override string? GetTextResource(string path) {
            foreach (Assembly assembly in _assemblies) {
                string[] resourceNames = assembly.GetManifestResourceNames();

                string pathWithLeadingPeriod = "." + path;
                string[] resourcePaths = resourceNames
                    .Where(x => x.EndsWith(pathWithLeadingPeriod, StringComparison.CurrentCultureIgnoreCase))
                    .ToArray();

                if (!resourcePaths.Any())
                    return null;

                if (resourcePaths.Count() > 1)
                    throw new Exception(
                        $"Multiple resources ending with {path} found: {Environment.NewLine}{string.Join(Environment.NewLine, resourcePaths)}");

                Stream stream = assembly.GetManifestResourceStream(resourcePaths.Single());
                if (stream == null)
                    return null;

                using (var streamReader = new StreamReader(stream)) {
                    return streamReader.ReadToEnd();
                }
            }

            return null;
        }

        public override IEnumerable<string> GetResources() {
            throw new NotImplementedException();
        }

        public override string RootPath => _assemblies.Count == 1 ? _assemblies[0].FullName : "assemblies";
    }
}
