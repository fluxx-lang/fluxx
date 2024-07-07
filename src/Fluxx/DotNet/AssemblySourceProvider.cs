using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Fluxx.SourceProviders;

namespace Fluxx.DotNet
{
    public class AssemblySourceProvider : SourceProvider
    {
        private readonly List<Assembly> assemblies;

        public AssemblySourceProvider(Assembly assembly)
        {
            this.assemblies = new List<Assembly> {assembly};
        }

        public override string? GetTextResource(string path)
        {
            foreach (Assembly assembly in this.assemblies)
            {
                string[] resourceNames = assembly.GetManifestResourceNames();

                string pathWithLeadingPeriod = "." + path;
                string[] resourcePaths = resourceNames
                    .Where(x => x.EndsWith(pathWithLeadingPeriod, StringComparison.CurrentCultureIgnoreCase))
                    .ToArray();

                if (!resourcePaths.Any())
                {
                    return null;
                }

                if (resourcePaths.Count() > 1)
                {
                    throw new Exception(
                        $"Multiple resources ending with {path} found: {Environment.NewLine}{string.Join(Environment.NewLine, resourcePaths)}");
                }

                Stream stream = assembly.GetManifestResourceStream(resourcePaths.Single());
                if (stream == null)
                {
                    return null;
                }

                using (var streamReader = new StreamReader(stream))
                {
                    return streamReader.ReadToEnd();
                }
            }

            return null;
        }

        public override IEnumerable<string> GetResources()
        {
            throw new NotImplementedException();
        }

        public override string RootPath => this.assemblies.Count == 1 ? this.assemblies[0].FullName : "assemblies";
    }
}
