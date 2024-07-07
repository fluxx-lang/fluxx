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
        private readonly ParseableSource _parseableSource;
        private readonly SourceText _sourceText;
        private readonly int _sourceEnd;
        private int _position;

        public LiteralValueCharIterator(SourceText sourceText, TextSpan sourceSpan)
        {
            this._sourceText = sourceText;

            this._position = sourceSpan.Start;
            this._sourceEnd = sourceSpan.End;
        }

        public char GetLookahead() => this._position < this._sourceEnd ? this._sourceText[this._position] : '\0';

        public char GetLookaheadPlusOne()
        {
            int positionPlusOne = this._position + 1;
            return positionPlusOne < this._sourceEnd ? this._sourceText[positionPlusOne] : '\0';
        }

        public char GetLookaheadAt(int index)
        {
            int lookaheadPosition = this._position + index;
            return lookaheadPosition < this._sourceEnd ? this._sourceText[lookaheadPosition] : '\0';
        }

        public int Position
        {
            get => this._position;
            set => this._position = value;
        }
        
        public char ReadChar()
        {
            return this._position >= this._sourceEnd ? '\0' : this._sourceText[this._position++];
        }

        public int GetMatchingRightBrace()
        {
            int nestedBraces = 0;
            int position = this._position;
            while (position < this._sourceEnd)
            {
                char currChar = this._sourceText[position];

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
                        return position;
                    else
                    {
                        --nestedBraces;
                        ++position;
                    }
                }
                else ++position;
            }

            return -1;
        }
    }
}
