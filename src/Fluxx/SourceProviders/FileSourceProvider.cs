using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;

namespace Faml.SourceProviders
{
    public class FileSourceProvider : SourceProvider
    {
        private readonly string _rootDirectory;


        public FileSourceProvider(string rootDirectory)
        {
            this._rootDirectory = rootDirectory;
        }

        public override string? GetTextResource(string path)
        {
            try
            {
                return File.ReadAllText(Path.Combine(this._rootDirectory, path));
            }
            catch (FileNotFoundException e)
            {
                return null;
            }
        }

        public override ImmutableArray<byte>? GetBinaryResource(string path)
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(Path.Combine(this._rootDirectory, path));
                return bytes.ToImmutableArray();
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        public override IEnumerable<string> GetResources()
        {
            int rootDirectoryLength = this._rootDirectory.Length;

            var resources = new List<string>();
            foreach (string filePath in Directory.EnumerateFiles(this._rootDirectory, "*.*", SearchOption.AllDirectories))
            {
                if (!filePath.StartsWith(this._rootDirectory))
                    throw new Exception(
                        $"Path {filePath} unexpectedly doesn't start with root directory {this._rootDirectory}");

                string relativePath = filePath.Substring(rootDirectoryLength);
                resources.Add(relativePath);
            }

            return resources;
        }

        public override string RootPath => this._rootDirectory;
    }
}
