using System.Collections.Generic;
using System.Collections.Immutable;

namespace TypeTooling.ClassifiedText
{
    public class ClassifiedTextElement
    {
        public const string TextClassificationTypeName = "text";

        public ClassifiedTextElement(params ClassifiedTextRun[] runs)
        {
            Runs = runs.ToImmutableArray();
        }

        public ClassifiedTextElement(IEnumerable<ClassifiedTextRun> runs)
        {
            Runs = runs.ToImmutableArray();
        }

        public ImmutableArray<ClassifiedTextRun> Runs { get; }
    }
}
