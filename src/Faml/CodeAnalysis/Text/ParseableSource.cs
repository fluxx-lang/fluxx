using System;
using Microsoft.CodeAnalysisP.Text;

namespace Faml.CodeAnalysis.Text {
    public class ParseableSource {
        private readonly SourceText _sourceText;
        private readonly int _startPosition;
        private readonly int _endPosition;


        public static bool IsLetter(char character)
            => (character >= 'a' && character <= 'z') || (character >= 'A' && character <= 'Z');

        public static bool IsSpace(char character) => character == ' ' || character == '\t' || character == '\r';

        public static bool IsNewline(char character) => character == '\n';

        public static bool IsSpaceOrNewline(char character) => IsSpace(character) || IsNewline(character);

        public static bool IsDigit(char character) => character >= '0' && character <= '9';

        public ParseableSource(SourceText sourceText, int startPosition, int endPosition) {
            _sourceText = sourceText;
            _startPosition = startPosition;
            _endPosition = endPosition;
        }

        public ParseableSource(SourceText sourceText) : this(sourceText, 0, sourceText.Length) { }

        public ParseableSource(SourceText sourceText, TextSpan span) : this(sourceText, span.Start, span.End) { }

        public SourceText SourceText => _sourceText;

        public int StartPosition => _startPosition;

        public int EndPosition => _endPosition;

        public int GetPrevNonSpace(int position) {
            int testPosition = position;
            while (true) {
                --testPosition;
                if (testPosition < _startPosition)
                    return testPosition;

                char currChar = _sourceText[testPosition];
                if (!IsSpace(currChar))
                    return testPosition;
            }
        }

        public int GetNextNonSpace(int position) {
            int testPosition = position;
            while (true) {
                ++testPosition;
                if (testPosition >= _endPosition)
                    return testPosition;

                char currChar = _sourceText[testPosition];
                if (!IsSpace(currChar))
                    return testPosition;
            }
        }

        public char GetCharAt(int position) => position < _startPosition || position >= _endPosition ? '\0' : _sourceText[position];

        public string Substring(int position, int length) {
            if (position < _startPosition)
                throw new ArgumentException("Substring position is before start of ParseableSource");
            if (position + length > _endPosition)
                throw new ArgumentException("Substring length is extends past end of ParseableSource");

            return _sourceText.ToString(new TextSpan(position, length));
        }

        public bool IsSpanSoleItemOnLine(TextSpan span) {
            if (!IsAtStartOfLine(span.Start))
                return false;

            int nextPosition = GetNextNonSpace(span.End - 1);
            char nextChar = GetCharAt(nextPosition);
            if (nextChar != '\n' && nextChar != '\0')
                return false;

            return true;
        }

        /// <summary>
        /// See if the specified position is at the beginning of the line, ignoring whitespace.
        /// </summary>
        /// <param name="position">position in question</param>
        /// <returns>true if the position only has whitespace before it on the line</returns>
        public bool IsAtStartOfLine(int position) {
            int prevPosition = GetPrevNonSpace(position);
            char prevChar = GetCharAt(prevPosition);

            return prevChar == '\n' || prevChar == '\0';
        }

        /// <summary>
        /// Check if position is indented, column-wise, relative to basePosition and on a line that comes after the line holding basePosition.
        /// </summary>
        /// <param name="basePosition">starting position</param>
        /// <param name="position">position to check</param>
        /// <returns>true iff position is on line and column that come after basePosition</returns>
        public bool IsIndentedOnSubsequentLine(int basePosition, int position) {
            if (position <= basePosition)
                return false;

            // Ensure the position is on a subsequent line
            int baseEndOfLine = GetEndOfLine(basePosition);
            if (position <= baseEndOfLine)
                return false;

            // Ensure that position is the first non-whitespace thing on the line
            if (!IsAtStartOfLine(position))
                return false;

            // Ensure that indent exceeds the base indent
            int column = GetColumn(position);
            int baseColumn = GetColumn(basePosition);
            return column > baseColumn;
        }

        /// <summary>
        /// Given a position, return the position of the end of the current line. The end of line has a '\n' character or is the end of the stream.
        /// </summary>
        /// <param name="position"></param>
        /// <returns>position of the end of the line</returns>
        public int GetEndOfLine(int position) {
            for (int currPosition = position; true; ++currPosition) {
                char currChar = GetCharAt(currPosition);
                if (currChar == '\n' || currChar == '\0')
                    return currPosition;
            }
        }

        public bool IsSpaceAt(int position) => IsSpace(GetCharAt(position));

        public bool IsNewlineAt(int position) => IsNewline(GetCharAt(position));

        public bool IsSpaceOrNewlineAt(int position) => IsSpaceOrNewline(GetCharAt(position));

        public int GetColumn(int position) => _sourceText.Lines.GetLinePosition(position).Character;
    }
}
