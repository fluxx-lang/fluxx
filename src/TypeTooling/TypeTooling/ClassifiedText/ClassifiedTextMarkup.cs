namespace TypeTooling.ClassifiedText
{
    public class ClassifiedTextMarkup
    {
        public ClassifiedTextMarkup(ContainerElement containerElement)
        {
            this.ContainerElement = containerElement;
        }

        public ClassifiedTextMarkup(ClassifiedTextElement classifiedTextElement)
            : this(new ContainerElement(ContainerElementStyle.Wrapped, classifiedTextElement))
        {
        }

        public ClassifiedTextMarkup(string text)
            : this(new ClassifiedTextElement(new ClassifiedTextRun(ClassifiedTextElement.TextClassificationTypeName, text)))
        {
        }

        public ContainerElement ContainerElement { get; }
    }
}
