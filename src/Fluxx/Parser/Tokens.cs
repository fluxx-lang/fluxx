using Faml.Lexer;

namespace Faml.Parser {
    public class Tokens {
        private readonly TokenType _tokenType;
        private readonly int _maxColumn;
        private readonly Tokens? _restTokens;

        public Tokens(TokenType tokenType, int maxColumn = -1) {
            _tokenType = tokenType;
            _maxColumn = maxColumn;
            _restTokens = null;
        }

        public Tokens(TokenType tokenType, Tokens restTokens) {
            _tokenType = tokenType;
            _maxColumn = -1;
            _restTokens = restTokens;
        }

        public Tokens(TokenType tokenType, int maxColumn, Tokens restTokens) {
            _tokenType = tokenType;
            _maxColumn = maxColumn;
            _restTokens = restTokens;
        }

        public Tokens Add(TokenType tokenType, int maxColumn = -1) {
            return new Tokens(tokenType, maxColumn, this);
        }

        public Tokens Add(Tokens tokens) {
            Tokens newTokens = this;

            Tokens remainingTokens = tokens;
            while (remainingTokens != null) {
                newTokens = newTokens.Add(remainingTokens._tokenType, remainingTokens._maxColumn);
                remainingTokens = remainingTokens._restTokens;
            }
            return newTokens;
        }

        public bool Contains(TokenType tokenType, int column) {
            Tokens tokens = this;
            while (tokens != null) {
                if (tokens._tokenType == tokenType && (tokens._maxColumn == -1 || column <= tokens._maxColumn))
                    return true;

                tokens = tokens._restTokens;
            }

            return false;
        }
    }
}
