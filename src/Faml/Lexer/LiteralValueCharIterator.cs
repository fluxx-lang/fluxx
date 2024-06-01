/**
 * @author Bret Johnson
 * @since 4/7/2014 6:17 PM
 */

using Microsoft.CodeAnalysisP.Text;
using Faml.CodeAnalysis.Text;

namespace Faml.Lexer {
    public sealed class LiteralValueCharIterator {
        private readonly ParseableSource _parseableSource;
        private readonly SourceText _sourceText;
        private readonly int _sourceEnd;
        private int _position;

        public LiteralValueCharIterator(SourceText sourceText, TextSpan sourceSpan) {
            _sourceText = sourceText;

            _position = sourceSpan.Start;
            _sourceEnd = sourceSpan.End;
        }

        public char GetLookahead() => _position < _sourceEnd ? _sourceText[_position] : '\0';

        public char GetLookaheadPlusOne() {
            int positionPlusOne = _position + 1;
            return positionPlusOne < _sourceEnd ? _sourceText[positionPlusOne] : '\0';
        }

        public char GetLookaheadAt(int index) {
            int lookaheadPosition = _position + index;
            return lookaheadPosition < _sourceEnd ? _sourceText[lookaheadPosition] : '\0';
        }

        public int Position {
            get => _position;
            set => _position = value;
        }
        
        public char ReadChar() {
            return _position >= _sourceEnd ? '\0' : _sourceText[_position++];
        }

        public int GetMatchingRightBrace() {
            int nestedBraces = 0;
            int position = _position;
            while (position < _sourceEnd) {
                char currChar = _sourceText[position];

                if (currChar == '\\') {
                    position += 2;
                }
                else if (currChar == '{') {
                    ++position;
                    ++nestedBraces;
                }
                else if (currChar == '}') {
                    if (nestedBraces == 0)
                        return position;
                    else {
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
