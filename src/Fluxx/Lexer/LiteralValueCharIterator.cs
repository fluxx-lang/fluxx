/**
 * @author Bret Johnson
 * @since 4/7/2014 6:17 PM
 */
using Faml.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Lexer
{
    public sealed class LiteralValueCharIterator
    {
        private readonly ParseableSource parseableSource;
        private readonly SourceText sourceText;
        private readonly int sourceEnd;
        private int position;

        public LiteralValueCharIterator(SourceText sourceText, TextSpan sourceSpan)
        {
            this.sourceText = sourceText;

            this.position = sourceSpan.Start;
            this.sourceEnd = sourceSpan.End;
        }

        public char GetLookahead() => this.position < this.sourceEnd ? this.sourceText[this.position] : '\0';

        public char GetLookaheadPlusOne()
        {
            int positionPlusOne = this.position + 1;
            return positionPlusOne < this.sourceEnd ? this.sourceText[positionPlusOne] : '\0';
        }

        public char GetLookaheadAt(int index)
        {
            int lookaheadPosition = this.position + index;
            return lookaheadPosition < this.sourceEnd ? this.sourceText[lookaheadPosition] : '\0';
        }

        public int Position
        {
            get => this.position;
            set => this.position = value;
        }
        
        public char ReadChar()
        {
            return this.position >= this.sourceEnd ? '\0' : this.sourceText[this.position++];
        }

        public int GetMatchingRightBrace()
        {
            int nestedBraces = 0;
            int position = this.position;
            while (position < this.sourceEnd)
            {
                char currChar = this.sourceText[position];

                if (currChar == '\\')
                {
                    position += 2;
                }
                else if (currChar == '{')
                {
                    ++position;
                    ++nestedBraces;
                }
                else if (currChar == '}')
                {
                    if (nestedBraces == 0)
                    {
                        return position;
                    }
                    else
                    {
                        --nestedBraces;
                        ++position;
                    }
                }
                else
                {
                    ++position;
                }
            }

            return -1;
        }
    }
}
