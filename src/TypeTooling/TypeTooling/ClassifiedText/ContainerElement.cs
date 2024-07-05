using System.Collections.Generic;
using System.Collections.Immutable;

namespace TypeTooling.ClassifiedText
{
    public class ContainerElement
    {
        public ContainerElement(ContainerElementStyle style, IEnumerable<object> elements)
        {
            this.Style = style;
            this.Elements = elements.ToImmutableArray();
        }

        public ContainerElement(ContainerElementStyle style, params object[] elements)
        {
            this.Style = style;
            this.Elements = elements.ToImmutableArray();
        }

        public ContainerElementStyle Style { get; }

        public ImmutableArray<object> Elements { get; }
    }
}
