using System.Collections.Generic;

namespace Fluxx.SourceProviders
{
    public class NullSourceProvider : SourceProvider
    {
        public NullSourceProvider() : base()
        {
        }

        public override string? GetTextResource(string path)
        {
            return null;
        }

        public override IEnumerable<string> GetResources()
        {
            return new List<string>();
        }

        public override string RootPath => "null-provider";
    }
}
