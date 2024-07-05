namespace Faml.Lexer {
    public enum ArgumentLookaheadType {
        DoubleColon,
        PropertyIdentifier,
        IndentedContent,
        None
    }

    public class ArgumentLookahead {
        private readonly ArgumentLookaheadType _type;
        private readonly int _indent;

        public ArgumentLookahead(ArgumentLookaheadType type) {
             _type = type;
            _indent = -1;
        }

        public ArgumentLookahead(int indent) {
            _type = ArgumentLookaheadType.IndentedContent;
            _indent = indent;
        }

        public ArgumentLookaheadType Type => _type;

        /// <summary>
        /// Get the indent amount (zero based), which is only valid for type INDENTED_CONTENT.
        /// </summary>
        public int Indent => _indent;
    }
}
