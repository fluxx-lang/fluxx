using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Fluxx.Api.IntelliSense
{
    [Serializable]
    public class IntelliSenseCompletions
    {
        public static IntelliSenseCompletions Empty = new IntelliSenseCompletions(Enumerable.Empty<IntelliSenseCompletion>());

        public ImmutableArray<IntelliSenseCompletion> Completions { get; }

        public IntelliSenseCompletions(IEnumerable<IntelliSenseCompletion> completions)
        {
            this.Completions = completions.ToImmutableArray();
        }
    }
}
