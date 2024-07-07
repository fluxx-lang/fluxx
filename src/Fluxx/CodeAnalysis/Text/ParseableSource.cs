using System;
using Microsoft.CodeAnalysis.Text;

namespace Fluxx.CodeAnalysis.Text
{
    public class ParseableSource
    {
        private readonly SourceText sourceText;
        private readonly int startPosition;
        private readonly int endPosition;

        public static bool IsLetter(char character)
            => (character >= 'a' && character <= 'z') || (character >= 'A' && character <= 'Z');

        public static bool IsSpace(char character) => character == ' ' || character == '\t' || character == '\r';

        public static bool IsNewline(char character) => character == '\n';

        public static bool IsSpaceOrNewline(char character) => IsSpace(character) || IsNewline(character);

        public static bool IsDigit(char character) => character >= '0' && character <= '9';

        public ParseableSource(SourceText sourceText, int startPosition, int endPosition)
        {
            this.sourceText = sourceText;
            this.startPosition = startPosition;
            this.endPosition = endPosition;
        }

        public ParseableSource(SourceText sourceText) : this(sourceText, 0, sourceText.Length) { }

        public ParseableSource(SourceText sourceText, TextSpan span) : this(sourceText, span.Start, span.End) { }

        public SourceText SourceText => this.sourceText;

        public int StartPosition => this.startPosition;

        public int EndPosition => this.endPosition;

        public int GetPrevNonSpace(int position)
        {
            int testPosition = position;
            while (true)
            {
                --testPosition;
                if (testPosition < this.startPosition)
                {
                    return testPosition;
                }

                char currChar = this.sourceText[testPosition];
                if (!IsSpace(currChar))
                {
                    return testPosition;
                }
            }
        }

        public int GetNextNonSpace(int position)
        {
            int testPosition = position;
            while (true)
            {
                ++testPosition;
                if (testPosition >= this.endPosition)
                {
                    return testPosition;
                }

                char currChar = this.sourceText[testPosition];
                if (!IsSpace(currChar))
                {
                    return testPosition;
                }
            }
        }

        public char GetCharAt(int position) => position < this.startPosition || position >= this.endPosition ? '\0' : this.sourceText[position];

        public string Substring(int position, int length)
        {
            if (position < this.startPosition)
            {
                throw new ArgumentException("Substring position is before start of ParseableSource");
            }

            if (position + length > this.endPosition)
            {
                throw new ArgumentException("Substring length is extends past end of ParseableSource");
            }

            return this.sourceText.ToString(new TextSpan(position, length));
        }

        public bool IsSpanSoleItemOnLine(TextSpan span)
        {
            if (!this.IsAtStartOfLine(span.Start))
            {
                return false;
            }

            int nextPosition = this.GetNextNonSpace(span.End - 1);
            char nextChar = this.GetCharAt(nextPosition);
            if (nextChar != '\n' && nextChar != '\0')
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// See if the specified position is at the beginning of the line, ignoring whitespace.
        /// </summary>
        /// <param name="position">position in question</param>
        /// <returns>true if the position only has whitespace before it on the line</returns>
        public bool IsAtStartOfLine(int position)
        {
            int prevPosition = this.GetPrevNonSpace(position);
            char prevChar = this.GetCharAt(prevPosition);

            return prevChar == '\n' || prevChar == '\0';
        }

        /// <summary>
        /// Check if position is indented, column-wise, relative to basePosition and on a line that comes after the line holding basePosition.
        /// </summary>
        /// <param name="basePosition">starting position</param>
        /// <param name="position">position to check</param>
        /// <returns>true iff position is on line and column that come after basePosition</returns>
        public bool IsIndentedOnSubsequentLine(int basePosition, int position)
        {
            if (position <= basePosition)
            {
                return false;
            }

            // Ensure the position is on a subsequent line
            int baseEndOfLine = this.GetEndOfLine(basePosition);
            if (position <= baseEndOfLine)
            {
                return false;
            }

            // Ensure that position is the first non-whitespace thing on the line
            if (!this.IsAtStartOfLine(position))
            {
                return false;
            }

            // Ensure that indent exceeds the base indent
            int column = this.GetColumn(position);
            int baseColumn = this.GetColumn(basePosition);
            return column > baseColumn;
        }

        /// <summary>
        /// Given a position, return the position of the end of the current line. The end of line has a '\n' character or is the end of the stream.
        /// </summary>
        /// <param name="position"></param>
        /// <returns>position of the end of the line</returns>
        public int GetEndOfLine(int position)
        {
            for (int currPosition = position; true; ++currPosition)
            {
                char currChar = this.GetCharAt(currPosition);
                if (currChar == '\n' || currChar == '\0')
                {
                    return currPosition;
                }
            }
        }

        public bool IsSpaceAt(int position) => IsSpace(this.GetCharAt(position));

        public bool IsNewlineAt(int position) => IsNewline(this.GetCharAt(position));

        public bool IsSpaceOrNewlineAt(int position) => IsSpaceOrNewline(this.GetCharAt(position));

        public int GetColumn(int position) => this.sourceText.Lines.GetLinePosition(position).Character;
    }
}
