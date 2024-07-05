using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Faml.Api.IntelliSense {
    [Serializable]
    public class IntelliSenseCompletions {
        public static IntelliSenseCompletions Empty = new IntelliSenseCompletions(Enumerable.Empty<IntelliSenseCompletion>());

        public ImmutableArray<IntelliSenseCompletion> Completions { get; }

        public IntelliSenseCompletions(IEnumerable<IntelliSenseCompletion> completions) {
            Completions = completions.ToImmutableArray();
        }
    }
}
