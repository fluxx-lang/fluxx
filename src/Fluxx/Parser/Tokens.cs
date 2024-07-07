using Faml.Lexer;

namespace Faml.Parser {
    public class Tokens {
        private readonly TokenType _tokenType;
        private readonly int _maxColumn;
        private readonly Tokens? _restTokens;

        public Tokens(TokenType tokenType, int maxColumn = -1) {
            this._tokenType = tokenType;
            this._maxColumn = maxColumn;
            this._restTokens = null;
        }

        public Tokens(TokenType tokenType, Tokens restTokens) {
            this._tokenType = tokenType;
            this._maxColumn = -1;
            this._restTokens = restTokens;
        }

        public Tokens(TokenType tokenType, int maxColumn, Tokens restTokens) {
            this._tokenType = tokenType;
            this._maxColumn = maxColumn;
            this._restTokens = restTokens;
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
                {
                    return true;
                }

                tokens = tokens._restTokens;
            }

            return false;
        }
    }
}
