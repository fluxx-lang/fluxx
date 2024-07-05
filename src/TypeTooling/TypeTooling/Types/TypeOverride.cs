namespace TypeTooling.Types
{
    public class TypeOverride
    {
        private readonly TypeToolingType _originalType;
        private Override<CustomLiteralParser>? _customLiteralParserOverride;
        private Override<Visualizer>? _visualizerOverride;


        public TypeOverride(TypeToolingType originalType)
        {
            _originalType = originalType;
        }

        public void OverrideCustomLiteralParser(CustomLiteralParser customLiteralParser)
        {
            _customLiteralParserOverride = new Override<CustomLiteralParser>(customLiteralParser);
        }

        public void OverrideVisualizer(Visualizer visualizer)
        {
            _visualizerOverride = new Override<Visualizer>(visualizer);
        }

        public CustomLiteralParser? GetCustomLiteralParser()
        {
            if (_customLiteralParserOverride != null)
                return _customLiteralParserOverride.Value;
            return _originalType.GetCustomLiteralParser();
        }

        public Visualizer? GetVisualizer()
        {
            if (_visualizerOverride != null)
                return _visualizerOverride.Value;
            return _originalType.GetVisualizer();
        }

        public class Override<T>
        {
            private readonly T _value;

            public Override(T value)
            {
                _value = value;
            }

            public T Value => _value;
        }
    }
}
