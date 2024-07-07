using Fluxx.Lexer;

namespace Fluxx.Parser
{
    public class Tokens
    {
        private readonly TokenType tokenType;
        private readonly int maxColumn;
        private readonly Tokens? restTokens;

        public Tokens(TokenType tokenType, int maxColumn = -1)
        {
            this.tokenType = tokenType;
            this.maxColumn = maxColumn;
            this.restTokens = null;
        }

        public Tokens(TokenType tokenType, Tokens restTokens)
        {
            this.tokenType = tokenType;
            this.maxColumn = -1;
            this.restTokens = restTokens;
        }

        public Tokens(TokenType tokenType, int maxColumn, Tokens restTokens)
        {
            this.tokenType = tokenType;
            this.maxColumn = maxColumn;
            this.restTokens = restTokens;
        }

        public Tokens Add(TokenType tokenType, int maxColumn = -1)
        {
            return new Tokens(tokenType, maxColumn, this);
        }

        public Tokens Add(Tokens tokens)
        {
            Tokens newTokens = this;

            Tokens remainingTokens = tokens;
            while (remainingTokens != null)
            {
                newTokens = newTokens.Add(remainingTokens.tokenType, remainingTokens.maxColumn);
                remainingTokens = remainingTokens.restTokens;
            }

            return newTokens;
        }

        public bool Contains(TokenType tokenType, int column)
        {
            Tokens tokens = this;
            while (tokens != null)
            {
                if (tokens.tokenType == tokenType && (tokens.maxColumn == -1 || column <= tokens.maxColumn))
                {
                    return true;
                }

                tokens = tokens.restTokens;
            }

            return false;
        }
    }
}
