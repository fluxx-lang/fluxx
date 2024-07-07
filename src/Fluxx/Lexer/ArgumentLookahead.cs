namespace Fluxx.Lexer
{
    public enum ArgumentLookaheadType
    {
        DoubleColon,
        PropertyIdentifier,
        IndentedContent,
        None
    }

    public class ArgumentLookahead
    {
        private readonly ArgumentLookaheadType type;
        private readonly int indent;

        public ArgumentLookahead(ArgumentLookaheadType type)
        {
             this.type = type;
            this.indent = -1;
        }

        public ArgumentLookahead(int indent)
        {
            this.type = ArgumentLookaheadType.IndentedContent;
            this.indent = indent;
        }

        public ArgumentLookaheadType Type => this.type;

        /// <summary>
        /// Get the indent amount (zero based), which is only valid for type INDENTED_CONTENT.
        /// </summary>
        public int Indent => this.indent;
    }
}
