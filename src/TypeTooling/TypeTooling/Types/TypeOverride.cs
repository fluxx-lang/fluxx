namespace TypeTooling.Types
{
    public class TypeOverride
    {
        private readonly TypeToolingType _originalType;
        private Override<CustomLiteralParser>? _customLiteralParserOverride;
        private Override<Visualizer>? _visualizerOverride;


        public TypeOverride(TypeToolingType originalType)
        {
            this._originalType = originalType;
        }

        public void OverrideCustomLiteralParser(CustomLiteralParser customLiteralParser)
        {
            this._customLiteralParserOverride = new Override<CustomLiteralParser>(customLiteralParser);
        }

        public void OverrideVisualizer(Visualizer visualizer)
        {
            this._visualizerOverride = new Override<Visualizer>(visualizer);
        }

        public CustomLiteralParser? GetCustomLiteralParser()
        {
            if (this._customLiteralParserOverride != null)
                return this._customLiteralParserOverride.Value;
            return this._originalType.GetCustomLiteralParser();
        }

        public Visualizer? GetVisualizer()
        {
            if (this._visualizerOverride != null)
                return this._visualizerOverride.Value;
            return this._originalType.GetVisualizer();
        }

        public class Override<T>
        {
            private readonly T _value;

            public Override(T value)
            {
                this._value = value;
            }

            public T Value => this._value;
        }
    }
}
