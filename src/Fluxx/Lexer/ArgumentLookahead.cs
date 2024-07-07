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
             this._type = type;
            this._indent = -1;
        }

        public ArgumentLookahead(int indent) {
            this._type = ArgumentLookaheadType.IndentedContent;
            this._indent = indent;
        }

        public ArgumentLookaheadType Type => this._type;

        /// <summary>
        /// Get the indent amount (zero based), which is only valid for type INDENTED_CONTENT.
        /// </summary>
        public int Indent => this._indent;
    }
}
