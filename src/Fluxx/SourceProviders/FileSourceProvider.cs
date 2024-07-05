using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;

namespace Faml.SourceProviders {
    public class FileSourceProvider : SourceProvider {
        private readonly string _rootDirectory;


        public FileSourceProvider(string rootDirectory) {
            _rootDirectory = rootDirectory;
        }

        public override string? GetTextResource(string path) {
            try {
                return File.ReadAllText(Path.Combine(_rootDirectory, path));
            }
            catch (FileNotFoundException e) {
                return null;
            }
        }

        public override ImmutableArray<byte>? GetBinaryResource(string path) {
            try {
                byte[] bytes = File.ReadAllBytes(Path.Combine(_rootDirectory, path));
                return bytes.ToImmutableArray();
            }
            catch (FileNotFoundException) {
                return null;
            }
        }

        public override IEnumerable<string> GetResources() {
            int rootDirectoryLength = _rootDirectory.Length;

            var resources = new List<string>();
            foreach (string filePath in Directory.EnumerateFiles(_rootDirectory, "*.*", SearchOption.AllDirectories)) {
                if (!filePath.StartsWith(_rootDirectory))
                    throw new Exception(
                        $"Path {filePath} unexpectedly doesn't start with root directory {_rootDirectory}");

                string relativePath = filePath.Substring(rootDirectoryLength);
                resources.Add(relativePath);
            }

            return resources;
        }

        public override string RootPath => _rootDirectory;
    }
}
