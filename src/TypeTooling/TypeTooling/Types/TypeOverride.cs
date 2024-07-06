namespace TypeTooling.Types
{
    public class TypeOverride
    {
        private readonly TypeToolingType originalType;
        private Override<CustomLiteralParser>? customLiteralParserOverride;
        private Override<Visualizer>? visualizerOverride;

        public TypeOverride(TypeToolingType originalType)
        {
            this.originalType = originalType;
        }

        public void OverrideCustomLiteralParser(CustomLiteralParser customLiteralParser)
        {
            this.customLiteralParserOverride = new Override<CustomLiteralParser>(customLiteralParser);
        }

        public void OverrideVisualizer(Visualizer visualizer)
        {
            this.visualizerOverride = new Override<Visualizer>(visualizer);
        }

        public CustomLiteralParser? GetCustomLiteralParser()
        {
            if (this.customLiteralParserOverride != null)
            {
                return this.customLiteralParserOverride.Value;
            }

            return this.originalType.GetCustomLiteralParser();
        }

        public Visualizer? GetVisualizer()
        {
            if (this.visualizerOverride != null)
            {
                return this.visualizerOverride.Value;
            }

            return this.originalType.GetVisualizer();
        }

        public class Override<T>
        {
            private readonly T value;

            public Override(T value)
            {
                this.value = value;
            }

            public T Value => this.value;
        }
    }
}
